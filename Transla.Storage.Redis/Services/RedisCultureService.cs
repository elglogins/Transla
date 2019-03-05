using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Transla.Contracts;
using Transla.Core.Interfaces.Services;
using Transla.Storage.Redis.Interfaces.Services;

namespace Transla.Storage.Redis.Services
{
    internal class RedisCultureService : ICultureService
    {
        private const string CulturesContainerKey = "Transla.AllowedCultures";

        private readonly IRedisConnectionProvider _redisConnectionProvider;

        public RedisCultureService(IRedisConnectionProvider redisConnectionProvider)
        {
            _redisConnectionProvider = redisConnectionProvider;
        }

        public async Task<IEnumerable<CultureContract>> GetAll()
        {
            var cultures = await _redisConnectionProvider.GetDatabase().HashGetAllAsync(CulturesContainerKey);
            return cultures.Select(s => new CultureContract()
            {
                CultureName = s.Name
            });
        }

        public async Task<CultureContract> Get(string cultureName)
        {
            if (String.IsNullOrWhiteSpace(cultureName))
            {
                throw new ArgumentNullException(nameof(cultureName));
            }

            var data = await _redisConnectionProvider.GetDatabase().HashGetAsync(CulturesContainerKey, cultureName);
            if (String.IsNullOrWhiteSpace(data))
                return null;

            return JsonConvert.DeserializeObject<CultureContract>(data);
        }

        public async Task Save(string cultureName)
        {
            if (String.IsNullOrWhiteSpace(cultureName))
            {
                throw new ArgumentNullException(nameof(cultureName));
            }

            var model = new CultureContract()
            {
                CultureName = cultureName
            };
            await _redisConnectionProvider.GetDatabase()
               .HashSetAsync(CulturesContainerKey, cultureName, JsonConvert.SerializeObject(model));
        }

        public async Task Delete(string cultureName)
        {
            if (String.IsNullOrWhiteSpace(cultureName))
            {
                throw new ArgumentNullException(nameof(cultureName));
            }

            var existing = await Get(cultureName);
            if (existing == null)
                throw new Exception("Not existing culture");

            await _redisConnectionProvider.GetDatabase()
               .HashDeleteAsync(CulturesContainerKey, cultureName);
        }
    }
}
