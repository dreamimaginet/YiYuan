using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using YiYuan.Entity;
using YiYuan.Extensions;
using YiYuan.Enums;
using YiYuan.CreateIssueBusiness.Service;

namespace YiYuan.CreateIssueBusiness
{
    public class CreateIssueBusiness
    {
        public static LogFactory log = new LogFactory("CreateIssue");

        private static readonly Object lockObj = new Object();

        private static Int32 threadCount = Convert.ToInt32(ConfigurationManager.AppSettings["threadCount"]);

        private static Int32 time = Convert.ToInt32(ConfigurationManager.AppSettings["time"]);

        /// <summary>
        /// 启动服务
        /// </summary>
        public static void Statr()
        {

            Int32 count = 0;

            while (count < threadCount)
            {
                new Thread(new CreateIssueBusiness().Create)
                {
                    IsBackground = true

                }.Start();

                count += 1;
            }

            log.Warn("自动生成期号服务已启动");
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public static void Stop()
        {
            log.Warn("自动生成期号服务已停止");
        }

        /// <summary>
        /// 自动生成期号
        /// </summary>
        public void Create()
        {
            List<UserOrder> orderList = new List<UserOrder>();

            List<Issue> needList = new List<Issue>();

            List<Issue> issueList = new List<Issue>();

            ActivityGoods activityGoods = new ActivityGoods();

            Activity activity = new Activity();

            Issue issue = null;

            int count = 0;

            TimeSpan timeA, timeB, timeC, timeD;

            try
            {
                while (true)
                {
                    // 查询投注已经满人次，但状态并没有截止的奖期

                    lock (lockObj)
                    {
                        try
                        {
                            needList = ServicesBusiness<Issue>.Where(w => w.IssueStatus == IssueStatus.Ongoing);
                        }
                        catch
                        {
                            Thread.Sleep(time);

                            continue;
                        }

                        if (needList == null || needList.Count == 0)
                        {
                            Thread.Sleep(time);

                            continue;
                        }

                        foreach (var item in needList)
                        {
                            timeA = DateTime.Now.TimeOfDay.Subtract(Convert.ToDateTime("22:00:00").TimeOfDay);

                            timeB = Convert.ToDateTime("01:55:00").TimeOfDay.Subtract(DateTime.Now.TimeOfDay);

                            timeC = DateTime.Now.TimeOfDay.Subtract(Convert.ToDateTime("01:55:00").TimeOfDay);

                            timeD = Convert.ToDateTime("10:00:00").TimeOfDay.Subtract(DateTime.Now.TimeOfDay);

                            #region 将投注满的期号的状态改为已截止

                            item.IssueStatus = IssueStatus.WaitLottery;

                            // 如果截止时间在 22：00：00 到第二天凌晨 01：55：00 这个区间里，那么就是5分钟开奖时间

                            if (timeA.TotalSeconds > 0 && timeB.TotalSeconds > 0)
                            {
                                item.EndTime = DateTime.Now.AddMinutes(5);
                            }
                            else
                            {
                                // 如果截止时间在 01：55：00 到当天上午 10：00：00 这个区间里，那么就是当天 10：05：00 统一开奖

                                if (timeC.TotalSeconds > 0 && timeD.TotalSeconds > 0)
                                {
                                    item.EndTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 10:05:00");
                                }
                                else
                                {
                                    item.EndTime = DateTime.Now.AddMinutes(10);
                                }
                            }

                            ServicesBusiness<Issue>.Update(item);

                            log.Info("第" + item.No + "期已成功截止");

                            String numberA = String.Empty;

                            orderList = ServicesBusiness<UserOrder>.WhereAndOrder(w => w.OrderStatus == OrderStatus.AlreadyPaid, o => o.CreateTime,CodeOrderType.Desc, s => s).ToList();

                            if (orderList.Count < 50)
                            {
                                log.Error("计算幸运号码 A 出错，出错原因：订单表不够 50 条数据，出错期号：" + item.No + "，请手动处理");
                            }
                            else
                            {
                                numberA = orderList.Take(50).Select(s => Convert.ToInt64(s.CreateTime.ToString("HHmmssfff"))).Sum().ToString();

                                if (String.IsNullOrWhiteSpace(numberA))
                                {
                                    log.Error("计算幸运号码 A 出错，出错期号：" + item.No + "，请手动处理");
                                }
                                else
                                {
                                    log.Info(String.Format("第{0}期生成幸运号码 A 成功，幸运号码为{1}", item.No, numberA));
                                }
                            }

                            #endregion
                        }
                    }

                    foreach (var item in needList)
                    {

                        // 判断活动是否结束，如果不为正常，则不生成下一期

                        activityGoods = ServicesBusiness<ActivityGoods>.Where(w => w.Id == item.ActivityGoodsId).FirstOrDefault();

                        activity = ServicesBusiness<Activity>.Where(w => w.Id == activityGoods.ActivityId).FirstOrDefault();

                        if (activity.ActivityStatus != ActivityStatus.Ongoing)
                        {
                            continue;
                        }

                        #region 生成下一期并开始投注

                        // 判断是否下一期是否手动生成过

                        issueList = ServicesBusiness<Issue>.Where(w => w.IssueStatus == IssueStatus.NotStart && w.PNo == item.No);

                        if (issueList.Count == 0)
                        {

                            issue = new Issue
                            {
                                No = ServicesBusiness<Issue>.Where(w => true).Max(m => m.No) + 1,
                                ActivityGoodsId = item.ActivityGoodsId,
                                CreateTime = DateTime.Now,
                                EndNumber = item.EndNumber,
                                IssueStatus = IssueStatus.Ongoing,
                                PNo = item.No,
                                TotalPrice = item.TotalPrice
                            };

                            issue = ServicesBusiness<Issue>.Create(issue,out count);

                            if (count > 0)
                            {
                                log.Error(String.Format("生成下一期出错，期号：{0}，请管理员手动生成，并查明原因！", item.No));

                                continue;
                            }
                            else
                            {
                                log.Info("期号" + issue.No + "生成成功并开始投注");
                            }
                        }

                        if (issueList.Count > 0)
                        {
                            issue = issueList.FirstOrDefault();

                            if (issue.IssueStatus != IssueStatus.Suspend)
                            {
                                issue.IssueStatus = IssueStatus.Ongoing;

                                if (ServicesBusiness<Issue>.Update(issue, w => w.No == issue.No))
                                {
                                    log.Info("期号" + issue.No + "开始投注");
                                }
                                else
                                {
                                    log.Error("期号" + issue.No + "状态改为进行中时失败！");
                                }
                            }
                            else
                            {
                                log.Info("期号" + issue.No + "以暂停，请手动开始");
                            }
                        }

                        #endregion
                    }


                    #region 将活动商品的生成的第一期改为投注中

                    // 判断是否存在未开始的第一期

                    issueList = ServicesBusiness<Issue>.Where(w => w.IssueStatus == IssueStatus.NotStart && w.PNo == 0);

                    if (issueList.Count > 0)
                    {
                        // 将状态改为进行中

                        ServicesBusiness<Issue>.Update(s => new Issue { IssueStatus = IssueStatus.Ongoing }, w => w.IssueStatus == IssueStatus.NotStart && w.PNo == 0);
                    }

                    #endregion

                    Thread.Sleep(time);
                };
            }
            catch (Exception ex)
            {
                log.Error("服务出现异常，请手动处理完后，重启服务！异常信息：" + ex.Message);
            }
        }
    }
}
