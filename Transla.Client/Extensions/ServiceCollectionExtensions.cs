using Microsoft.Extensions.DependencyInjection;
using Transla.Client.Interfaces;
using Transla.Client.Services;

namespace Transla.Client.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTranslaConfiguration(this IServiceCollection services, ITranslaConfiguration configuration)
        {
            services.AddHttpClient();
            services.AddSingleton<ITranslaConfiguration>(configuration);
            services.AddTransient<IDictionaryService, TranslaDictionaryService>();
        }
    }
}
