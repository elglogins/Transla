using Microsoft.Extensions.DependencyInjection;
using System;
using Transla.Service.Interfaces.Services;
using Transla.Storage.Redis.Interfaces.Services;
using Transla.Storage.Redis.Services;

namespace Transla.Storage.Redis.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTranslaRedisStorage(this IServiceCollection services, IRedisConnectionProvider redisConnection)
        {
            if (redisConnection == null)
                throw new ArgumentNullException(nameof(redisConnection));

            services.AddSingleton<IRedisConnectionProvider>(redisConnection);
            AddDependencies(services);
        }

        public static void AddTranslaRedisStorage(this IServiceCollection services, string redisConnectionString, int redisDatabaseId)
        {
            if (String.IsNullOrWhiteSpace(redisConnectionString))
                throw new ArgumentNullException(nameof(redisConnectionString));

            services.AddSingleton<IRedisConnectionProvider>(new RedisConnectionProvider(redisConnectionString, redisDatabaseId));
            AddDependencies(services);
        }

        private static void AddDependencies(this IServiceCollection services)
        {
            services.AddTransient<ICultureService, RedisCultureService>();
            services.AddTransient<IDictionaryService, RedisDictionaryService>();
            services.AddTransient<IApplicationService, RedisApplicationService>();
            services.AddTransient<IApiKeyService, RedisApiKeyService>();
        }
    }
}
