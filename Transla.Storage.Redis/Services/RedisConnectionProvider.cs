using System;
using StackExchange.Redis;
using Transla.Storage.Redis.Interfaces.Services;

namespace Transla.Storage.Redis.Services
{
    internal class RedisConnectionProvider : IRedisConnectionProvider
    {
        protected string ConnectionString { get; set; }
        protected int DatabaseId { get; set; }
        protected int ConnectionTimeToLive { get; set; }
        private readonly object _connectionInitLock = new object();
        private ConnectionMultiplexer _connection;
        private DateTime _connectionDate;

        public RedisConnectionProvider(string connectionString, int databaseId)
        {
            if (String.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            ConnectionString = connectionString;
            ConnectionTimeToLive = 3600; // 1 hour
            DatabaseId = databaseId;
        }

        public ConnectionMultiplexer Connection
        {
            get
            {
#pragma warning disable DateTimeNowAnalyzer // DateTime UtcNow should be used for real time cache handling
                var now = DateTime.UtcNow;
#pragma warning restore DateTimeNowAnalyzer // DateTime static creation property should not be used directly
                if (_connection == null || !_connection.IsConnected || (now - _connectionDate).TotalSeconds > ConnectionTimeToLive)
                {
                    lock (_connectionInitLock)
                    {
                        if (_connection == null || !_connection.IsConnected || (now - _connectionDate).TotalSeconds > ConnectionTimeToLive)
                        {
                            _connection?.Close();
                            _connection = ConnectionMultiplexer.Connect(ConnectionString);
                            _connectionDate = now;
                        }
                    }
                }
                return _connection;
            }
        }

        public IDatabase GetDatabase() => Connection.GetDatabase(DatabaseId);

        public int GetDatabaseId() => DatabaseId;
    }
}
