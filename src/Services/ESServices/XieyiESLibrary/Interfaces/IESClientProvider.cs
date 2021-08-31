using Nest;

namespace XieyiESLibrary.Interfaces
{
    /// <summary>
    ///     初始化ES
    /// </summary>
    public interface IESClientProvider
    {
        ElasticClient ElasticClient { get; }
    }
}