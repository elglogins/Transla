using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Transla.Api.Contracts;
using Transla.Api.Interfaces.Services;

namespace Transla.Api.Services
{
    public class RedisDictionaryService : IDictionaryService
    {
        /// <summary>
        /// Transla:{serviceAlias}:{cultureName}:{code}
        /// </summary>
        private const string DictionariesPrefixFormat = "Transla:{0}:{1}:{2}";

        private readonly IRedisConnectionProvider _redisConnectionProvider;
        protected readonly int DatabaseId;

        public RedisDictionaryService(IRedisConnectionProvider redisConnectionProvider)
        {
            _redisConnectionProvider = redisConnectionProvider;
            DatabaseId = 5;
        }

        public async Task Delete(string cultureName, string service, string alias)
        {
            if (String.IsNullOrWhiteSpace(service))
                throw new ArgumentNullException(nameof(service));

            if (String.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            if (String.IsNullOrWhiteSpace(cultureName))
                throw new ArgumentNullException(nameof(cultureName));

            var existing = await Get(service, alias, cultureName);
            if (existing == null)
                throw new Exception("Item doesn't exist");

            await _redisConnectionProvider.GetDatabase(DatabaseId)
                .KeyDeleteAsync(String.Format(DictionariesPrefixFormat, service, cultureName, alias));
        }

        public async Task<IEnumerable<DictionaryContract>> Get(string service, string alias)
        {
            if (String.IsNullOrWhiteSpace(service))
                throw new ArgumentNullException(nameof(service));

            if (String.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            // get the target server
            var server = _redisConnectionProvider.Connection.GetServer(_redisConnectionProvider.Connection.GetEndPoints().First());
            var keys = server.Keys(pattern: $"Transla:{service}:*:{alias}");
            var results = new List<DictionaryContract>(keys.Count());
            foreach (var key in keys)
            {
                var data = await _redisConnectionProvider.GetDatabase(DatabaseId).StringGetAsync(key);
                results.Add(JsonConvert.DeserializeObject<DictionaryContract>(data));
            }

            return results;
        }

        public async Task<DictionaryContract> Get(string service, string alias, string cultureName)
        {
            if (String.IsNullOrWhiteSpace(service))
                throw new ArgumentNullException(nameof(service));

            if (String.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            if (String.IsNullOrWhiteSpace(cultureName))
                throw new ArgumentNullException(nameof(cultureName));

            var data = await _redisConnectionProvider.GetDatabase(DatabaseId)
                .StringGetAsync(String.Format(DictionariesPrefixFormat, service, cultureName, alias));

            if (String.IsNullOrWhiteSpace(data))
                return null;

            return JsonConvert.DeserializeObject<DictionaryContract>(data);
        }

        public async Task<IEnumerable<DictionaryContract>> GetAll()
        {
            // get the target server
            var server = _redisConnectionProvider.Connection.GetServer(_redisConnectionProvider.Connection.GetEndPoints().First());
            var keys = server.Keys(pattern: "Transla:*");
            var results = new List<DictionaryContract>(keys.Count());
            foreach(var key in keys)
            {
                var data = await _redisConnectionProvider.GetDatabase(DatabaseId).StringGetAsync(key);
                results.Add(JsonConvert.DeserializeObject<DictionaryContract>(data));
            }

            return results;
        }

        public async Task<IEnumerable<DictionaryContract>> GetAll(string service)
        {
            if (String.IsNullOrWhiteSpace(service))
                throw new ArgumentNullException(nameof(service));

            // get the target server
            var server = _redisConnectionProvider.Connection.GetServer(_redisConnectionProvider.Connection.GetEndPoints().First());
            var keys = server.Keys(pattern: $"Transla:{service}:*");
            var results = new List<DictionaryContract>(keys.Count());
            foreach (var key in keys)
            {
                var data = await _redisConnectionProvider.GetDatabase(DatabaseId).StringGetAsync(key);
                results.Add(JsonConvert.DeserializeObject<DictionaryContract>(data));
            }

            return results;
        }

        public async Task<IEnumerable<DictionaryContract>> GetAll(string service, string cultureName)
        {
            if (String.IsNullOrWhiteSpace(cultureName))
                throw new ArgumentNullException(nameof(cultureName));

            if (String.IsNullOrWhiteSpace(service))
                throw new ArgumentNullException(nameof(service));

            // get the target server
            var server = _redisConnectionProvider.Connection.GetServer(_redisConnectionProvider.Connection.GetEndPoints().First());
            var keys = server.Keys(pattern: $"Transla:{service}:{cultureName}:*");
            var results = new List<DictionaryContract>(keys.Count());
            foreach (var key in keys)
            {
                var data = await _redisConnectionProvider.GetDatabase(DatabaseId).StringGetAsync(key);
                results.Add(JsonConvert.DeserializeObject<DictionaryContract>(data));
            }

            return results;
        }

        public async Task Save(DictionaryContract contract)
        {
            if (contract == null)
                throw new ArgumentNullException(nameof(contract));

            if (String.IsNullOrWhiteSpace(contract.Alias))
                throw new ArgumentNullException(nameof(contract.Alias));

            if (String.IsNullOrWhiteSpace(contract.CultureName))
                throw new ArgumentNullException(nameof(contract.CultureName));

            if (String.IsNullOrWhiteSpace(contract.Service))
                throw new ArgumentNullException(nameof(contract.Service));

            await _redisConnectionProvider.GetDatabase(DatabaseId)
                .StringSetAsync(String.Format(DictionariesPrefixFormat, contract.Service, contract.CultureName, contract.Alias), contract.Value);
        }
    }
}
