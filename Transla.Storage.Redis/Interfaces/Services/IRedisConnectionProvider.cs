using StackExchange.Redis;

namespace Transla.Storage.Redis.Interfaces.Services
{
    public interface IRedisConnectionProvider
    {
        ConnectionMultiplexer Connection { get; }

        IDatabase GetDatabase();
        int GetDatabaseId();
    }
}
