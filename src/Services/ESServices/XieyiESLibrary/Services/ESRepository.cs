using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nest;
using XieyiESLibrary.ExpressionsToQuery;
using XieyiESLibrary.Extensions;
using XieyiESLibrary.Interfaces;

namespace XieyiESLibrary.Services
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

        public async Task<bool> InsertAsync<T>(T entity, string index = "") where T : class
        {
            try
            {
                var indexName = index.GetIndex<T>();
                var exist = await IndexExistsAsync(indexName);
                if (!exist) await ((ElasticClient)_elasticClient).CreateIndexAsync<T>(indexName);
                var response = await _elasticClient.IndexAsync(entity, x => x.Index(indexName)).ConfigureAwait(false);
                if (!response.IsValid)
                    throw new Exception(
                        $"add entity into index: [{indexName}] fail :{response.OriginalException.Message}");
                return response.IsValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> InsertRangeAsync<T>(IEnumerable<T> entities, string index = "") where T : class
        {
            try
            {
                var indexName = index.GetIndex<T>();
                var exist = await IndexExistsAsync(indexName);
                if (!exist)
                {
                    await ((ElasticClient)_elasticClient).CreateIndexAsync<T>(indexName).ConfigureAwait(false);
                    await AddAliasAsync(indexName, typeof(T).Name);
                }

                var bulkRequest = new BulkRequest(indexName)
                {
                    Operations = new List<IBulkOperation>()
                };
                var operations = entities.Select(o => new BulkIndexOperation<T>(o)).Cast<IBulkOperation>().ToList();
                bulkRequest.Operations = operations;
                var response = await _elasticClient.BulkAsync(bulkRequest).ConfigureAwait(false);

                if (!response.IsValid)
                    throw new Exception($"addRange entities into index: [{indexName}] fail :" +
                                        response.OriginalException.Message);
                return response.IsValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> IndexExistsAsync(string index = "")
        {
            var result = await _elasticClient.Indices.ExistsAsync(index);
            return result.Exists;
        }

        public async Task<bool> DeleteIndexAsync<T>(string index = "") where T : class
        {
            try
            {
                var indexName = index.GetIndex<T>();
                var exists = await IndexExistsAsync(indexName);
                if (!exists)
                    throw new Exception($" index: [{indexName}] not exists");

                var response = await _elasticClient.Indices.DeleteAsync(indexName).ConfigureAwait(false);
                if (!response.IsValid)
                    throw new Exception($"delete index: [{indexName}] fail:" + response.OriginalException.Message);
                return response.IsValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> DeleteEntityByIdAsync<T>(string id, string index = "") where T : class
        {
            try
            {
                var indexName = index.GetIndex<T>();
                var exists = await IndexExistsAsync(indexName);
                if (!exists)
                    throw new Exception($"delete entity fail, because index:[{indexName}] is not found");

                var documentPath = new DocumentPath<T>(id);
                var response = await _elasticClient.DeleteAsync(documentPath, x => x.Index(indexName))
                    .ConfigureAwait(false);
                if (!response.IsValid)
                    throw new Exception("delete entity fail :" + response.OriginalException.Message);
                return response.IsValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> DeleteManyAsync<T>(List<T> deleteEntity) where T : class
        {
            try
            {
                var indexName = string.Empty.GetIndex<T>();
                var exists = await IndexExistsAsync(indexName);
                if (!exists)
                    throw new Exception($"delete entity fail, because index:[{indexName}] is not found");

                var response = await _elasticClient.DeleteManyAsync(deleteEntity, indexName).ConfigureAwait(false);
                return response.IsValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> DeleteByQuery<T>(Expression<Func<T, bool>> expression, string index = "")
            where T : class, new()
        {
            try
            {
                var indexName = index.GetIndex<T>();
                var request = new DeleteByQueryRequest<T>(indexName);
                var build = new QueryBuilder<T>();
                request.Query = build.GetQueryContainer(expression);
                var response = await _elasticClient.DeleteByQueryAsync(request).ConfigureAwait(false);
                if (!response.IsValid)
                    throw new Exception("delete fail:" + response.OriginalException.Message);
                return response.IsValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> UpdateAsync<T>(string key, T entity, string index = "") where T : class
        {
            try
            {
                var indexName = index.GetIndex<T>();
                var request = new UpdateRequest<T, object>(indexName, key)
                {
                    Doc = entity
                };
                var response = await _elasticClient.UpdateAsync(request).ConfigureAwait(false);
                if (!response.IsValid)
                    throw new Exception("update entity fail :" + response.OriginalException.Message);
                return response.IsValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return false;
            }
        }

        public async Task<BulkAliasResponse> AddAliasAsync(string index, string alias)
        {
            try
            {
                var response = await _elasticClient.Indices.BulkAliasAsync(b => b.Add(al => al
                    .Index(index)
                    .Alias(alias)));

                if (!response.IsValid)
                    throw new Exception($"add Alias:[{alias}] on index:[{index}] fail:" +
                                        response.OriginalException.Message);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return Activator.CreateInstance<BulkAliasResponse>();
            }
        }

        public BulkAliasResponse RemoveAlias(string index, string alias)
        {
            try
            {
                var response = _elasticClient.Indices.BulkAlias(b => b.Remove(al => al
                    .Index(index)
                    .Alias(alias)));

                if (!response.IsValid && response.ApiCall.HttpStatusCode != (int)HttpStatusCode.NotFound)
                    throw new Exception($"remove Alias:[{alias}] on index:[{index}] fail:" +
                                        response.OriginalException?.Message);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return Activator.CreateInstance<BulkAliasResponse>();
            }
        }

        public async Task<BulkAliasResponse> AddAliasAsync<T>(string alias) where T : class
        {
            return await AddAliasAsync(string.Empty.GetIndex<T>(), alias);
        }

        public BulkAliasResponse RemoveAlias<T>(string alias) where T : class
        {
            return RemoveAlias(string.Empty.GetIndex<T>(), alias);
        }
    }
}