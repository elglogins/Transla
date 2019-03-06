using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string PresenceCacheKey = "Transla.Dictionaries";
        private readonly object dictionariesLock = new object();

        public TranslaDictionaryService(IHttpClientFactory httpClientFactory, IMemoryCache cache, ITranslaConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Get(string alias)
        {
            var culture = _configuration.DefaultCulture;
            var request = _httpContextAccessor.HttpContext.Request;
            // query string is top priority
            if (request.Query != null && request.Query.ContainsKey("culture"))
            {
                culture = request.Query["culture"].ToString();
            }
            else if (request.Headers["Culture"].FirstOrDefault() != null)
            {
                culture = request.Headers["Culture"].ToString();
            }

            return Get(alias, culture);
        }

        public string Get(string alias, string cultureName)
        {
            EnsureCacheUpdated();
            return _cache.Get<DictionaryContract>(GenerateKey(alias, cultureName))?.Value ?? alias;
        }

        private void EnsureCacheUpdated()
        {
            var existingInCache = _cache.Get<int?>(PresenceCacheKey);
            if (!existingInCache.HasValue
                || existingInCache != 1)
            {
                lock (dictionariesLock)
                {
                    // make sure it is still not in cache
                    var existingInCache2 = _cache.Get<int?>(PresenceCacheKey);
                    if (!existingInCache2.HasValue
                        || existingInCache2 != 1)
                    {
                        try
                        {
                            // reload
                            var items = LoadDictionaryItems().GetAwaiter().GetResult();
                            Recache(items);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        private void Recache(IEnumerable<DictionaryContract> dictionaries)
        {
            _cache.Set<int?>(PresenceCacheKey, 1, new DateTimeOffset(DateTime.Now.AddMinutes(_configuration.CacheExpirationInMinutes)));
            foreach (var dictionary in dictionaries)
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
            client.Timeout = TimeSpan.FromSeconds(15);
            var response = await client.GetAsync($"{_configuration.BaseAddress}/api/dictionary/{_configuration.ApplicationAlias}");
            response.EnsureSuccessStatusCode();
            var responseText = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<DictionaryContract>>(responseText);
        }
    }
}
