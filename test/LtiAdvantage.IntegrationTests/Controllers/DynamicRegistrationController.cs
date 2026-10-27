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
        public DynamicRegistrationController(IWebHostEnvironment env, ILogger<DynamicRegistrationControllerBase> logger)
            : base(env, logger) { }

        protected override Task<ActionResult<ToolConfiguration>> OnRegisterAsync(RegisterToolRequest request)
        {
            // Lets tests check that an error result from the override is not turned into 201 Created.
            if (request.Tool.ClientName == "reject")
                return Task.FromResult<ActionResult<ToolConfiguration>>(
                    new ObjectResult(new { error = "invalid_redirect_uri" }));

            request.Tool.ClientId = Guid.NewGuid().ToString();
            return Task.FromResult<ActionResult<ToolConfiguration>>(
                new ObjectResult(request.Tool) { StatusCode = 201 });
        }
    }
}
