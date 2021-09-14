using StackExchange.Redis;

namespace XieyiRedisLibrary.Interfaces
{
    public interface IRedisProvider
    {
        public ConnectionMultiplexer RedisMultiplexer { get; set; }
    }
}