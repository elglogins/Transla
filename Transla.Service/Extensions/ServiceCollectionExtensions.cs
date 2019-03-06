using Microsoft.Extensions.DependencyInjection;
using Transla.Service.Controllers;

namespace Transla.Service.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTransla(this IServiceCollection services)
        {
            services.AddMvc()
                  .AddApplicationPart(typeof(ApplicationController).Assembly);
        }
    }
}
