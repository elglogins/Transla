using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transla.Api.Contracts;
using Transla.Api.Interfaces.Services;

namespace Transla.Api.Services
{
    public class RedisCultureService : ICultureService
    {
        private const string CulturesContainerKey = "Transla.AllowedCultures";

        private readonly IRedisConnectionProvider _redisConnectionProvider;
        protected readonly int DatabaseId;
        

        public RedisCultureService(IRedisConnectionProvider redisConnectionProvider)
        {
            _redisConnectionProvider = redisConnectionProvider;
            DatabaseId = 5;
        }

        public async Task<IEnumerable<CultureContract>> GetAll()
        {
            var cultures = await _redisConnectionProvider.GetDatabase(DatabaseId).HashGetAllAsync(CulturesContainerKey);
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

            var data = await _redisConnectionProvider.GetDatabase(DatabaseId).HashGetAsync(CulturesContainerKey, cultureName);
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
            await _redisConnectionProvider.GetDatabase(DatabaseId)
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

            await _redisConnectionProvider.GetDatabase(DatabaseId)
               .HashDeleteAsync(CulturesContainerKey, cultureName);
        }
    }
}
