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

        // The client name selects a behavior, so tests can exercise how the base handles each result.
        protected override Task<ActionResult<ToolConfiguration>> OnRegisterAsync(RegisterToolRequest request)
        {
            switch (request.Tool.ClientName)
            {
                case "reject":
                    // An error without a status code must not go out as 200 OK.
                    return Result(new ObjectResult(new { error = "invalid_redirect_uri" }));
                case "ok-without-client-id":
                    return Result(Ok(request.Tool));
                case "ok":
                    request.Tool.ClientId = Guid.NewGuid().ToString();
                    return Result(Ok(request.Tool));
                default:
                    request.Tool.ClientId = Guid.NewGuid().ToString();
                    return Result(new ObjectResult(request.Tool) { StatusCode = 201 });
            }
        }

        private static Task<ActionResult<ToolConfiguration>> Result(ActionResult result) =>
            Task.FromResult<ActionResult<ToolConfiguration>>(result);
    }
}
