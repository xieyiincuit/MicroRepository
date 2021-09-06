using System;
using System.Threading.Tasks;
using Nest;

namespace XieyiESLibrary.Extensions
{
    public static class ESClientExtension
    {
        /// <summary>
        ///     创建索引
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elasticClient"></param>
        /// <param name="indexName"></param>
        /// <param name="numberOfShards">副本数量 默认为5</param>
        /// <param name="numberOfReplicas">分片 默认为1</param>
        /// <returns></returns>
        public static async Task CreateIndexAsync<T>(
            this ElasticClient elasticClient, string indexName = "",
            int numberOfShards = 5, int numberOfReplicas = 1) where T : class
        {
            if (string.IsNullOrWhiteSpace(indexName))
                throw new ArgumentException("indexName require a value, can't be empty.");

            //index not exist, create this index
            if (!(await elasticClient.Indices.ExistsAsync(indexName)).Exists)
            {
                //设置index相关参数
                var indexState = new IndexState
                {
                    Settings = new IndexSettings
                    {
                        NumberOfReplicas = numberOfReplicas,
                        NumberOfShards = numberOfShards,
                        RefreshInterval = TimeSpan.FromSeconds(1),
                        // index.blocks.read_only：设为true,则索引以及索引的元数据只可读
                        // index.blocks.read_only_allow_delete：设为true，只读时允许删除。
                        // index.blocks.read：设为true，则不可读。
                        // index.blocks.write：设为true，则不可写。
                        // index.blocks.metadata：设为true，则索引元数据不可读写
                    }
                };

                //Map一个空Document
                var response = await elasticClient.Indices.CreateAsync(indexName,
                    p => p.InitializeUsing(indexState).Map<T>(x => x.AutoMap()));

                if (!response.IsValid)
                    throw new Exception($"Create index:[{indexName}] failed! : {response.OriginalException.Message}");
            }
        }
    }
}