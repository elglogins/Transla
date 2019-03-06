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
    internal class RedisApplicationService : IApplicationService
    {
        private const string ApplicationsContainerKey = "Transla.Applications";

        private readonly IRedisConnectionProvider _redisConnectionProvider;

        public RedisApplicationService(IRedisConnectionProvider redisConnectionProvider)
        {
            _redisConnectionProvider = redisConnectionProvider;
        }
        public async Task Delete(string alias)
        {
            if (String.IsNullOrWhiteSpace(alias))
            {
                throw new ArgumentNullException(nameof(alias));
            }

            var existing = await Get(alias);
            if (existing == null)
                throw new Exception("Not existing application");

            await _redisConnectionProvider.GetDatabase()
               .HashDeleteAsync(ApplicationsContainerKey, alias);
        }

        public async Task<ApplicationContract> Get(string alias)
        {
            if (String.IsNullOrWhiteSpace(alias))
            {
                throw new ArgumentNullException(nameof(alias));
            }

            var data = await _redisConnectionProvider.GetDatabase().HashGetAsync(ApplicationsContainerKey, alias);
            if (String.IsNullOrWhiteSpace(data))
                return null;

            return JsonConvert.DeserializeObject<ApplicationContract>(data);
        }

        public async Task<IEnumerable<ApplicationContract>> GetAll()
        {
            var applications = await _redisConnectionProvider.GetDatabase().HashGetAllAsync(ApplicationsContainerKey);
            return applications.Select(s => new ApplicationContract()
            {
                Alias = s.Name
            });
        }

        public async Task Save(string alias)
        {
            if (String.IsNullOrWhiteSpace(alias))
            {
                throw new ArgumentNullException(nameof(alias));
            }

            var model = new ApplicationContract()
            {
                Alias = alias
            };
            await _redisConnectionProvider.GetDatabase()
               .HashSetAsync(ApplicationsContainerKey, alias, JsonConvert.SerializeObject(model));
        }
    }
}
