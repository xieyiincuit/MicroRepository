using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Nest;

namespace XieyiESLibrary.Interfaces
{
    /// <summary>
    ///     ES CUD Repository
    /// </summary>
    public interface IESRepository
    {
        #region Update Part

        /// <summary>
        ///     根据Id更新index中的Doc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">Id</param>
        /// <param name="entity"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync<T>(string key, T entity, string index = "") where T : class;

        #endregion

        #region Index Part

        /// <summary>
        ///     往Index中新增数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<bool> InsertAsync<T>(T entity, string index = "") where T : class;

        /// <summary>
        ///     Bulk批量新增
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<bool> InsertRangeAsync<T>(IEnumerable<T> entities, string index = "") where T : class;


        /// <summary>
        ///     判断索引是否存在
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<bool> IndexExistsAsync(string index);

        #endregion

        #region Delete Part

        /// <summary>
        ///     删除index指定符合条件的Doc
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<bool> DeleteEntityByIdAsync<T>(string id, string index = "") where T : class;

        /// <summary>
        ///     删除索引 (支持根据实体建立的索引删除)
        ///     T is entity for a index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<bool> DeleteIndexAsync<T>(string index = "") where T : class;

        /// <summary>
        ///     通过数据唯一标识批量删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deleteEntity"></param>
        /// <returns></returns>
        Task<bool> DeleteManyAsync<T>(List<T> deleteEntity) where T : class;

        /// <summary>
        ///     通过Linq查询删除实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        Task<bool> DeleteByQuery<T>(Expression<Func<T, bool>> expression, string index = "") where T : class, new();

        #endregion

        #region Alias Part

        /// <summary>
        ///     添加别名
        /// </summary>
        /// <param name="index"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        Task<BulkAliasResponse> AddAliasAsync(string index, string alias);

        /// <summary>
        ///     添加别名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="alias"></param>
        /// <returns></returns>
        Task<BulkAliasResponse> AddAliasAsync<T>(string alias) where T : class;

        /// <summary>
        ///     删除别名
        /// </summary>
        /// <param name="index"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        BulkAliasResponse RemoveAlias(string index, string alias);

        /// <summary>
        ///     删除别名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="alias"></param>
        /// <returns></returns>
        BulkAliasResponse RemoveAlias<T>(string alias) where T : class;

        #endregion
    }
}