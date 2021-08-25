using Nest;

namespace XieyiESLibrary.Provider.Base
{
    public interface IESClientProvider
    {
        ElasticClient ElasticClient { get; }
    }
}