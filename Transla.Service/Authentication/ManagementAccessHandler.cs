using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Transla.Service.Interfaces.Configurations;

namespace Transla.Service.Authentication
{
    internal class ManagementAccessHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IManagementConfiguration _managementConfiguration;
        private const string HeaderName = "apiKey";

        public ManagementAccessHandler(
           IManagementConfiguration managementConfiguration,
           IOptionsMonitor<AuthenticationSchemeOptions> options,
           ILoggerFactory logger,
           UrlEncoder encoder,
           ISystemClock clock)
           : base(options, logger, encoder, clock)
        {
            _managementConfiguration = managementConfiguration;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var requestHeaders = Context.Request.Headers;
            if (!requestHeaders.Keys.Contains(HeaderName, StringComparer.InvariantCultureIgnoreCase))
            {
                return AuthenticateResult.Fail("Invalid API key");
            }

            var headerValue = requestHeaders[HeaderName].FirstOrDefault();
            if (String.IsNullOrWhiteSpace(headerValue) || !_managementConfiguration.AdministrationApiKey.Equals(headerValue))
            {
                return AuthenticateResult.Fail("Invalid API key");
            }

            // success! Now we just need to create the auth ticket
            var identity = new ClaimsIdentity("ManagementApiKey"); // the name of our auth scheme
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), null, "ManagementApiKey");
            return AuthenticateResult.Success(ticket);
        }
    }
}
