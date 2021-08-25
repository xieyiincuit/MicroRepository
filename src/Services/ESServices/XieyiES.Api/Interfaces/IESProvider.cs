using Nest;

namespace XieyiES.Api.Interfaces
{
    /// <summary>
    /// ElasticSearch接口
    /// </summary>
    public interface IESProvider
    {
        /// <summary>
        /// High Level
        /// </summary>
        public ElasticClient ElasticClient { get; set; }
    }
}