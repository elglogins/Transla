using Microsoft.AspNetCore.Builder;

namespace Transla.Service.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseTransla(this IApplicationBuilder app)
        {
            app.UseCors("TranslaAllowAll");
        }
    }
}
