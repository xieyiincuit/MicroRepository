using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            try
            {
                var indexName = index.GetIndex<T>();
                var exist = await IndexExistsAsync(indexName);
                if (!exist) await ((ElasticClient) _elasticClient).CreateIndexAsync<T>(indexName);
                var response = await _elasticClient.IndexAsync(entity, x => x.Index(indexName));
                if (!response.IsValid)
                    throw new Exception(
                        $"add entity into index: [{indexName}] fail :{response.OriginalException.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
            }
        }

        public async Task InsertRangeAsync<T>(IEnumerable<T> entities, string index = "") where T : class
        {
            try
            {
                var indexName = index.GetIndex<T>();
                var exist = await IndexExistsAsync(indexName);
                if (!exist)
                {
                    await ((ElasticClient) _elasticClient).CreateIndexAsync<T>(indexName);
                    await AddAliasAsync(indexName, typeof(T).Name);
                }

                var bulkRequest = new BulkRequest(indexName)
                {
                    Operations = new List<IBulkOperation>()
                };
                var operations = entities.Select(o => new BulkIndexOperation<T>(o)).Cast<IBulkOperation>().ToList();
                bulkRequest.Operations = operations;
                var response = await _elasticClient.BulkAsync(bulkRequest);

                if (!response.IsValid)
                    throw new Exception($"addRange entities into index: [{indexName}] fail :" +
                                        response.OriginalException.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
            }
        }

        public async Task<bool> IndexExistsAsync(string index)
        {
            var result = await _elasticClient.Indices.ExistsAsync(index);
            return result.Exists;
        }

        public async Task DeleteIndexAsync<T>() where T : class
        {
            try
            {
                var indexName = string.Empty.GetIndex<T>();
                var exists = await IndexExistsAsync(indexName);
                if (!exists)
                    return;

                var response = await _elasticClient.Indices.DeleteAsync(indexName);
                if (!response.IsValid)
                    throw new Exception($"delete index: [{indexName}] fail:" + response.OriginalException.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
            }
        }

        public async Task<DeleteResponse> DeleteEntityByIdAsync<T>(string id, string index = "") where T : class
        {
            try
            {
                var indexName = index.GetIndex<T>();
                var exists = await IndexExistsAsync(indexName);
                if (!exists)
                    throw new Exception($"delete entity fail, because index:[{indexName}] is not found");

                var request = new DocumentPath<T>(id);
                var response = await _elasticClient.DeleteAsync(request);
                if (!response.IsValid) throw new Exception("delete entity fail :" + response.OriginalException.Message);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return Activator.CreateInstance<DeleteResponse>();
            }
        }

        public async Task<IUpdateResponse<T>> UpdateAsync<T>(string key, T entity, string index = "") where T : class
        {
            try
            {
                var indexName = index.GetIndex<T>();
                var request = new UpdateRequest<T, object>(indexName, key)
                {
                    Doc = entity
                };

                var response = await _elasticClient.UpdateAsync(request);
                if (!response.IsValid)
                    throw new Exception("update entity fail :" + response.OriginalException.Message);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}{Environment.NewLine}Stack:{ex.StackTrace}");
                return Activator.CreateInstance<UpdateResponse<T>>();
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

        public async Task<BulkAliasResponse> AddAliasAsync<T>(string alias) where T : class
        {
            return await AddAliasAsync(string.Empty.GetIndex<T>(), alias);
        }

        public BulkAliasResponse RemoveAlias(string index, string alias)
        {
            try
            {
                var response = _elasticClient.Indices.BulkAlias(b => b.Remove(al => al
                    .Index(index)
                    .Alias(alias)));

                if (!response.IsValid && response.ApiCall.HttpStatusCode != (int) HttpStatusCode.NotFound)
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

        public BulkAliasResponse RemoveAlias<T>(string alias) where T : class
        {
            return RemoveAlias(string.Empty.GetIndex<T>(), alias);
        }
    }
}