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
    internal class RedisDictionaryService : IDictionaryService
    {
        /// <summary>
        /// Transla:{applicationAlias}:{cultureName}:{code}
        /// </summary>
        private const string DictionariesPrefixFormat = "Transla:{0}:{1}:{2}";

        private readonly IRedisConnectionProvider _redisConnectionProvider;

        public RedisDictionaryService(IRedisConnectionProvider redisConnectionProvider)
        {
            _redisConnectionProvider = redisConnectionProvider;
        }

        public async Task Delete(string cultureName, string application, string alias)
        {
            if (String.IsNullOrWhiteSpace(application))
                throw new ArgumentNullException(nameof(application));

            if (String.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            if (String.IsNullOrWhiteSpace(cultureName))
                throw new ArgumentNullException(nameof(cultureName));

            var existing = await Get(application, alias, cultureName);
            if (existing == null)
                throw new Exception("Item doesn't exist");

            await _redisConnectionProvider.GetDatabase()
                .KeyDeleteAsync(String.Format(DictionariesPrefixFormat, application, cultureName, alias));
        }

        public async Task<IEnumerable<DictionaryContract>> Get(string application, string alias)
        {
            if (String.IsNullOrWhiteSpace(application))
                throw new ArgumentNullException(nameof(application));

            if (String.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            // get the target server
            var server = _redisConnectionProvider.Connection.GetServer(_redisConnectionProvider.Connection.GetEndPoints().First());
            var keys = server.Keys(pattern: $"Transla:{application}:*:{alias}", database: _redisConnectionProvider.GetDatabaseId());
            var results = new List<DictionaryContract>(keys.Count());
            foreach (var key in keys)
            {
                var data = await _redisConnectionProvider.GetDatabase().StringGetAsync(key);
                results.Add(JsonConvert.DeserializeObject<DictionaryContract>(data));
            }

            return results;
        }

        public async Task<DictionaryContract> Get(string application, string alias, string cultureName)
        {
            if (String.IsNullOrWhiteSpace(application))
                throw new ArgumentNullException(nameof(application));

            if (String.IsNullOrWhiteSpace(alias))
                throw new ArgumentNullException(nameof(alias));

            if (String.IsNullOrWhiteSpace(cultureName))
                throw new ArgumentNullException(nameof(cultureName));

            var data = await _redisConnectionProvider.GetDatabase()
                .StringGetAsync(String.Format(DictionariesPrefixFormat, application, cultureName, alias));

            if (String.IsNullOrWhiteSpace(data))
                return null;

            return JsonConvert.DeserializeObject<DictionaryContract>(data);
        }

        public async Task<IEnumerable<DictionaryContract>> GetAll()
        {
            // get the target server
            var server = _redisConnectionProvider.Connection.GetServer(_redisConnectionProvider.Connection.GetEndPoints().First());
            var keys = server.Keys(pattern: "Transla:*", database: _redisConnectionProvider.GetDatabaseId()).ToList();
            var results = new List<DictionaryContract>(keys.Count());
            foreach(var key in keys)
            {
                var data = await _redisConnectionProvider.GetDatabase().StringGetAsync(key);
                results.Add(JsonConvert.DeserializeObject<DictionaryContract>(data));
            }

            return results;
        }

        public async Task<IEnumerable<DictionaryContract>> GetAll(string application)
        {
            if (String.IsNullOrWhiteSpace(application))
                throw new ArgumentNullException(nameof(application));

            // get the target server
            var server = _redisConnectionProvider.Connection.GetServer(_redisConnectionProvider.Connection.GetEndPoints().First());
            var keys = server.Keys(pattern: $"Transla:{application}:*", database: _redisConnectionProvider.GetDatabaseId());
            var results = new List<DictionaryContract>(keys.Count());
            foreach (var key in keys)
            {
                var data = await _redisConnectionProvider.GetDatabase().StringGetAsync(key);
                results.Add(JsonConvert.DeserializeObject<DictionaryContract>(data));
            }

            return results;
        }

        public async Task<IEnumerable<DictionaryContract>> GetAll(string application, string cultureName)
        {
            if (String.IsNullOrWhiteSpace(cultureName))
                throw new ArgumentNullException(nameof(cultureName));

            if (String.IsNullOrWhiteSpace(application))
                throw new ArgumentNullException(nameof(application));

            // get the target server
            var server = _redisConnectionProvider.Connection.GetServer(_redisConnectionProvider.Connection.GetEndPoints().First());
            var keys = server.Keys(pattern: $"Transla:{application}:{cultureName}:*", database: _redisConnectionProvider.GetDatabaseId());
            var results = new List<DictionaryContract>(keys.Count());
            foreach (var key in keys)
            {
                var data = await _redisConnectionProvider.GetDatabase().StringGetAsync(key);
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

            if (String.IsNullOrWhiteSpace(contract.Application))
                throw new ArgumentNullException(nameof(contract.Application));

            await _redisConnectionProvider.GetDatabase()
                .StringSetAsync(String.Format(DictionariesPrefixFormat, contract.Application, contract.CultureName, contract.Alias), JsonConvert.SerializeObject(contract));
        }
    }
}
