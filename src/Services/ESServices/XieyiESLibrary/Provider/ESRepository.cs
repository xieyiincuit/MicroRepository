using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;
using XieyiESLibrary.Extensions;
using XieyiESLibrary.Provider.Base;

namespace XieyiESLibrary.Provider
{
    public class ESRepository : IESRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<ESRepository> _logger;

        public ESRepository(IESClientProvider clientProvider, ILogger<ESRepository> logger)
        {
            _logger = logger;
            _elasticClient = clientProvider.ElasticClient;
        }

        public async Task InsertAsync<T>(T entity, string index = "") where T : class
        {
            var indexName = index.GetIndex<T>();
            bool exist = await IndexExistsAsync(indexName);
            if (!exist)
            {
                await ((ElasticClient)_elasticClient).CreateIndexAsync<T>(indexName);
            }

            var response = await _elasticClient.IndexAsync(entity, x => x.Index(index));

            if (!response.IsValid)
            {
                throw new Exception($"add entity fail! :{response.OriginalException.Message}");
            }

        }

        public async Task InsertRangeAsync<T>(IEnumerable<T> entities, string index = "") where T : class
        {
            throw new NotImplementedException();
        }

        public async Task BulkIndexAsync<T>(IEnumerable<T> entities, string index = "") where T : class
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IndexExistsAsync(string index)
        {
            var result = await _elasticClient.Indices.ExistsAsync(index);
            return result.Exists;
        }

        public async Task DeleteIndexAsync(string index)
        {
            throw new NotImplementedException();
        }

        public async Task<DeleteByQueryResponse> DeleteByQueryAsync<T>(Expression<Func<T, bool>> expression, string index = "") where T : class, new()
        {
            throw new NotImplementedException();
        }

        public async Task<IUpdateResponse<T>> UpdateAsync<T>(string key, T entity, string index = "") where T : class
        {
            throw new NotImplementedException();
        }

        public async Task<BulkAliasResponse> AddAliasAsync(string index, string alias)
        {
            throw new NotImplementedException();
        }

        public async Task<BulkAliasResponse> AddAliasAsync<T>(string alias) where T : class
        {
            throw new NotImplementedException();
        }

        public BulkAliasResponse RemoveAlias(string index, string alias)
        {
            throw new NotImplementedException();
        }

        public BulkAliasResponse RemoveAlias<T>(string alias) where T : class
        {
            throw new NotImplementedException();
        }
    }
}