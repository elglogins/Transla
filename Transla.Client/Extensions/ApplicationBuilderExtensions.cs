using Microsoft.AspNetCore.Builder;
using Transla.Client.Services;

namespace Transla.Client.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseTransla(this IApplicationBuilder app)
        {
            var dictionaryService = (IDictionaryService)app.ApplicationServices.GetService(typeof(IDictionaryService));
            // ensure that latest application dictionaries are loaded
            dictionaryService.Get("Hello","World");
        }
    }
}
