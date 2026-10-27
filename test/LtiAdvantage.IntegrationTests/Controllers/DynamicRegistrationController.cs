using System;
using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.DynamicRegistration;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class DynamicRegistrationController : DynamicRegistrationControllerBase
    {
        /// <summary>An opaque (non-JWT) registration token this test platform accepts.</summary>
        public const string OpaqueRegistrationToken = "opaque-single-use-token";

        public DynamicRegistrationController(IWebHostEnvironment env, ILogger<DynamicRegistrationControllerBase> logger)
            : base(env, logger) { }

        // Shows a platform accepting its own opaque token, and falling back to the default JWT check.
        protected override Task<ActionResult> AuthorizeRegistrationAsync(string registrationToken)
        {
            return registrationToken == OpaqueRegistrationToken
                ? Task.FromResult<ActionResult>(null)
                : base.AuthorizeRegistrationAsync(registrationToken);
        }

        // The client name selects a behavior, so tests can exercise how the base handles each outcome.
        protected override Task<ToolConfiguration> OnRegisterAsync(RegisterToolRequest request)
        {
            switch (request.Tool.ClientName)
            {
                case "reject":
                    throw new RegistrationRejectedException("invalid_redirect_uri", "Redirect URI not allowed.");
                case "return-null":
                    return Task.FromResult<ToolConfiguration>(null);
                case "keep-client-id":
                    // A careless override that only fills in a missing client_id.
                    request.Tool.ClientId ??= Guid.NewGuid().ToString();
                    return Task.FromResult(request.Tool);
                case "no-client-id":
                    return Task.FromResult(request.Tool);
                default:
                    request.Tool.ClientId = Guid.NewGuid().ToString();
                    return Task.FromResult(request.Tool);
            }
        }
    }
}
