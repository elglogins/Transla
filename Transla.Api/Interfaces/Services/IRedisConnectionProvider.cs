using StackExchange.Redis;

namespace Transla.Api.Interfaces.Services
{
    public interface IRedisConnectionProvider
    {
        ConnectionMultiplexer Connection { get; }

        IDatabase GetDatabase(int databaseId);
    }
}
