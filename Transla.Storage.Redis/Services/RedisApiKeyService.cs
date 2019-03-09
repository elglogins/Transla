using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transla.Contracts;
using Transla.Service.Interfaces.Services;
using Transla.Storage.Redis.Interfaces.Services;

namespace Transla.Storage.Redis.Services
{
    internal class RedisApiKeyService : IApiKeyService
    {
        private const string ApiKeysContainerKey = "Transla.ApiKeys";

        private readonly IRedisConnectionProvider _redisConnectionProvider;

        public RedisApiKeyService(IRedisConnectionProvider redisConnectionProvider)
        {
            _redisConnectionProvider = redisConnectionProvider;
        }

        public async Task Save(string apiKey, string applicationAlias)
        {
            if (String.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            if (String.IsNullOrWhiteSpace(applicationAlias))
            {
                throw new ArgumentNullException(nameof(applicationAlias));
            }

            var model = new ApiKeyContract()
            {
                ApplicationAlias = applicationAlias,
                Key = apiKey
            };
            await _redisConnectionProvider.GetDatabase()
               .HashSetAsync(ApiKeysContainerKey, apiKey, JsonConvert.SerializeObject(model));
        }

        public async Task Delete(string apiKey)
        {
            if (String.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            var existing = await Get(apiKey);
            if (existing == null)
                throw new Exception("Not existing application");

            await _redisConnectionProvider.GetDatabase()
               .HashDeleteAsync(ApiKeysContainerKey, apiKey);
        }

        public async Task<ApiKeyContract> Get(string apiKey)
        {
            if (String.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }

            var data = await _redisConnectionProvider.GetDatabase().HashGetAsync(ApiKeysContainerKey, apiKey);
            if (String.IsNullOrWhiteSpace(data))
                return null;

            return JsonConvert.DeserializeObject<ApiKeyContract>(data);
        }

        public async Task<IEnumerable<ApiKeyContract>> GetAll()
        {
            var apiKeys = await _redisConnectionProvider.GetDatabase().HashGetAllAsync(ApiKeysContainerKey);
            return apiKeys.Select(s => JsonConvert.DeserializeObject<ApiKeyContract>(s.Value));
        }

        public async Task<bool> IsValid(string apiKey)
        {
            return (await Get(apiKey)) != null;
        }
    }
}
