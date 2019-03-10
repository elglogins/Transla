using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Transla.Service.Authentication;
using Transla.Service.Configurations;
using Transla.Service.Controllers;
using Transla.Service.Interfaces.Configurations;

namespace Transla.Service.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddTransla(this IServiceCollection services, string administrationApiKey)
        {
            AddTransla(services, administrationApiKey, null);
        }

        public static void AddTransla(this IServiceCollection services, string administrationApiKey, string[] allowedCorsOrigins)
        {
            if (string.IsNullOrWhiteSpace(administrationApiKey))
                throw new ArgumentNullException(nameof(administrationApiKey));

            services.AddMvc()
                  .AddApplicationPart(typeof(ApplicationController).Assembly);

            if (allowedCorsOrigins == null || !allowedCorsOrigins.Any())
            {
                services.AddCors(options => options.AddPolicy("TranslaAllowAll", p => p.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()));
            }
            else
            {
                services.AddCors(options => options.AddPolicy("TranslaAllowAll", p => p.WithOrigins(allowedCorsOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()));
            }

            services.AddAuthentication(AuthenticationSchemas.ManagementAccess)
                .AddScheme<AuthenticationSchemeOptions, ManagementAccessHandler>(AuthenticationSchemas.ManagementAccess, null);

            services.AddSingleton<IManagementConfiguration>(new ManagementConfiguration()
            {
                AdministrationApiKey = administrationApiKey
            });
        }

    }
}
