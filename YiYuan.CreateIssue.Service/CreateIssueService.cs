using System.ServiceProcess;

namespace YiYuan.CreateIssueBusiness.Service
{
    partial class CreateIssueService : ServiceBase
    {
        public CreateIssueService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。

            CreateIssueBusiness.Statr();
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。

            CreateIssueBusiness.Stop();
        }
    }
}
