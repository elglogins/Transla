using Microsoft.Extensions.DependencyInjection;
using System;
using Transla.Service.Configurations;
using Transla.Service.Controllers;
using Transla.Service.Interfaces.Configurations;

namespace Transla.Service.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTransla(this IServiceCollection services, string administrationApiKey)
        {
            if (string.IsNullOrWhiteSpace(administrationApiKey))
                throw new ArgumentNullException(nameof(administrationApiKey));

            services.AddSingleton<IManagementConfiguration>(new ManagementConfiguration()
            {
                AdministrationApiKey = administrationApiKey
            });

            services.AddMvc()
                  .AddApplicationPart(typeof(ApplicationController).Assembly);

            services.AddCors(options => options.AddPolicy("TranslaAllowAll", p => p.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader()));
        }

    }
}
