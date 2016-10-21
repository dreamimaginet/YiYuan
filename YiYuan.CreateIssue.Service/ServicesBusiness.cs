using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using YiYuan.Business;

namespace YiYuan.CreateIssueBusiness.Service
{
    public class ServicesBusiness<T> where T : class
    {
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static List<T> Where(Expression<Func<T, bool>> where)
        {
            return new BaseBusiness<T>().GetByWhere(where).Data.ToList();
        }

        /// <summary>
        /// 查询并排序
        /// </summary>
        /// <param name="issue"></param>
        /// <returns></returns>
        public static List<TOut> WhereAndOrder<TOut, TKey>(Expression<Func<T, bool>> where, Expression<Func<T, TKey>> orderLambda, CodeOrderType codeOrderType, Expression<Func<T, TOut>> selectExpression)
        {
            return new BaseBusiness<T>().GetByWhere<TOut, TKey>(where, orderLambda, codeOrderType, selectExpression).Data.ToList();
        }

        /// <summary>
        /// 条件更新实体
        /// </summary>
        /// <param name="issue"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static bool Update(T entity, Expression<Func<T, bool>> where)
        {
            return new BaseBusiness<T>().Update(where, entity).Data;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="issue"></param>
        /// <returns></returns>
        public static void Update(T entity)
        {
            new BaseBusiness<T>().Update(entity);
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="updateExpression"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public static bool Update(Expression<Func<T, T>> updateExpression, Expression<Func<T, bool>> where)
        {
            return new BaseBusiness<T>().Update(where, updateExpression).Data;
        }

        /// <summary>
        /// 新增实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="isSubmit">是否马上提交</param>
        /// <returns></returns>
        public static T Create(T entity, bool isSubmit = true)
        {
            return new BaseBusiness<T>().Create(entity, isSubmit);
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">要新增的实体</param>
        /// <param name="IsSubmit">是否马上提交</param>
        /// <param name="rowNumber">受影响行数</param>
        /// <returns></returns>
        public static T Create(T entity, out int rowNumber, bool isSubmit = true)
        {
            return new BaseBusiness<T>().Create(entity, out rowNumber, isSubmit);
        }
    }
}
