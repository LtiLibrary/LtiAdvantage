using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
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
                        new ClaimsPrincipal(TestAuthOptions.GetIdentity(scope)),
                        new AuthenticationProperties(),
                        Scheme.Name)));
        }
    }

    public class TestAuthOptions : AuthenticationSchemeOptions
    {
        public static ClaimsIdentity GetIdentity(string scope)
        {
            var identity = new ClaimsIdentity(
                new[]
                {
                    new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                        Guid.NewGuid().ToString()),
                    new Claim("scope", scope)
                },
                "test");
            return identity;
        }
    }
}
