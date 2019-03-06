using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Transla.Client.Interfaces;
using Transla.Contracts;

namespace Transla.Client.Services
{
    class TranslaDictionaryService : IDictionaryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly ITranslaConfiguration _configuration;
        private const string PresenceCacheKey = "Transla.Dictionaries";

        public TranslaDictionaryService(IHttpClientFactory httpClientFactory, IMemoryCache cache, ITranslaConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<string> GetDictionaryValue(string alias, string cultureName)
        {
            var existingInCache = _cache.Get<int?>(PresenceCacheKey);

            // TODO: add lock
            if (!existingInCache.HasValue 
                || existingInCache != 1)
            {
                // reload
                var items = await LoadDictionaryItems();
                Recache(items);
            }

            return _cache.Get<DictionaryContract>(GenerateKey(alias, cultureName))?.Value ?? alias;
        }

        private void Recache(IEnumerable<DictionaryContract> dictionaries)
        {
            _cache.Set<int?>(PresenceCacheKey, 1, new DateTimeOffset(DateTime.Now.AddMinutes(_configuration.CacheExpirationInMinutes)));
            foreach(var dictionary in dictionaries)
            {
                _cache.Set<DictionaryContract>(GenerateKey(dictionary.Alias, dictionary.CultureName), dictionary);
            }
        }

        private string GenerateKey(string alias, string cultureName)
        {
            return $"Transla.{cultureName}.{alias}";
        }

        private async Task<IEnumerable<DictionaryContract>> LoadDictionaryItems()
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(20);
            var response = await client.GetAsync($"{_configuration.BaseAddress}/api/dictionary/{_configuration.ApplicationAlias}");
            response.EnsureSuccessStatusCode();
            var responseText = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<DictionaryContract>>(responseText);
        }
    }
}
