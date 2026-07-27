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
            request.Tool.ClientId = Guid.NewGuid().ToString();
            return Task.FromResult<ActionResult<ToolConfiguration>>(
                new ObjectResult(request.Tool) { StatusCode = 201 });
        }
    }
}
