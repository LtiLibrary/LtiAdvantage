using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LtiAdvantage.IntegrationTests
{
    public class TestAuthHandler : AuthenticationHandler<TestAuthOptions>
    {
        public TestAuthHandler(IOptionsMonitor<TestAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Headers.TryGetValue("x-test-scope", out var scope))
            {
                return Task.FromResult(AuthenticateResult.Fail("No x-test-scope header."));
            }

            if (string.IsNullOrWhiteSpace(scope))
            {
                return Task.FromResult(AuthenticateResult.Fail("Empty x-test-scope header."));
            }

            return Task.FromResult(
                AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(Options.GetIdentity(scope)),
                        new AuthenticationProperties(),
                        Scheme.Name)));
        }
    }

    public class TestAuthOptions : AuthenticationSchemeOptions
    {
        public ClaimsIdentity GetIdentity(string scope)
        {
            var identity = new ClaimsIdentity(
                new Claim[]
                {
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                        Guid.NewGuid().ToString()),
                    new Claim("http://schemas.microsoft.com/identity/claims/tenantid", "test"),
                    new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier",
                        Guid.NewGuid().ToString()),
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "test"),
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "test"),
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "test"),
                    new Claim("scope", scope)
                },
                "test");
            return identity;
        }
    }
}
