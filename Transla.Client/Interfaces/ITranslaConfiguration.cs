namespace Transla.Client.Interfaces
{
    public interface ITranslaConfiguration
    {
        string BaseAddress { get; set; }
        string ApplicationAlias { get; set; }
        int CacheExpirationInMinutes { get; set; }
        string DefaultCulture { get; set; }
    }
}
