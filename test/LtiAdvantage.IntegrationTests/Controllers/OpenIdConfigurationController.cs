using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.OpenIdConnect;
using LtiAdvantage.DynamicRegistration;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class OpenIdConfigurationController : OpenIdConfigurationControllerBase
    {
        public OpenIdConfigurationController(ILogger<OpenIdConfigurationControllerBase> logger) : base(logger)
        {
        }

        protected override Task<PlatformOpenIdConfiguration> OnGetOpenIdConfigurationAsync()
        {
            return Task.FromResult(new PlatformOpenIdConfiguration
            {
                Issuer = "https://platform.example.com",
                AuthorizationEndpoint = Url.Link("default", new {controller = "authorize"}),

                // Resolve the sibling endpoints through their named routes so the document
                // cannot drift from where the controllers actually live.
                JwksUri = Url.Link(Constants.ServiceEndpoints.Jwks.JwksService, null),
                RegistrationEndpoint = Url.Link("lti-dynamic-registration", null),

                TokenEndpointAuthMethodsSupported = new[] {"private_key_jwt"},
                ResponseTypesSupported = new[] {"id_token"},
                SubjectTypesSupported = new[] {"public"},
                IdTokenSigningAlgValuesSupported = new[] {"RS256"},
                ScopesSupported = new[]
                {
                    Constants.LtiScopes.Nrps.MembershipReadonly,
                    Constants.LtiScopes.Gs.ContextGroupReadonly,
                    Constants.LtiScopes.Ap.ControlAll
                },
                LtiPlatformConfiguration = new LtiPlatformConfiguration
                {
                    ProductFamilyCode = "ExamplePlatform",
                    Version = "1.0"
                }
            });
        }
    }
}
