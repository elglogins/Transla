using Transla.Client.Interfaces;

namespace Transla.Client
{
    public class TranslaConfiguration : ITranslaConfiguration
    {
        public string BaseAddress { get; set; }
        public string ApplicationAlias { get; set; }
        public int CacheExpirationInMinutes { get; set; }
        public string DefaultCulture { get; set; }
    }
}
