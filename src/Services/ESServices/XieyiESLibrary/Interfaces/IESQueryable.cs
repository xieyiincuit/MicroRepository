using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XieyiESLibrary.Entity;

namespace XieyiESLibrary.Interfaces
{
    /// <summary>
    ///     ES查询后数据处理接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IESQueryable<T>
    {
        /// <summary>
        ///     获取第一个元素
        /// </summary>
        /// <returns></returns>
        Task<T> FirstAsync();

        /// <summary>
        ///     同步返回列表
        /// </summary>
        /// <returns></returns>
        List<T> ToList();

        /// <summary>
        ///     异步返回列表
        /// </summary>
        /// <returns></returns>
        Task<List<T>> ToListAsync();

        /// <summary>
        ///     分页返回查询结果
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<T> ToPageList(int pageIndex, int pageSize);

        /// <summary>
        ///     分页返回查询结果（返回查询个数总数）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalNumber"></param>
        /// <returns></returns>
        List<T> ToPageList(int pageIndex, int pageSize, out long totalNumber);

        /// <summary>
        ///     筛选
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IESQueryable<T> Where(Expression<Func<T, bool>> expression);

        /// <summary>
        ///     排序
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="type">默认升序</param>
        /// <returns></returns>
        IESQueryable<T> OrderBy(Expression<Func<T, object>> expression, OrderByType type = OrderByType.Asc);

        /// <summary>
        ///     分组
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IESQueryable<T> GroupBy(Expression<Func<T, object>> expression);
    }
}