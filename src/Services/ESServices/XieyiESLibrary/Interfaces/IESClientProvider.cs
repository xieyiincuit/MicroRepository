using Nest;

namespace XieyiESLibrary.Interfaces
{
    public interface IESClientProvider
    {
        ElasticClient ElasticClient { get; }
    }
}