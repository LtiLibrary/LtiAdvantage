using System;
using System.Collections.Concurrent;
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
        private readonly TestRegistrationTokenStore _tokens;

        public DynamicRegistrationController(
            IWebHostEnvironment env, ILogger<DynamicRegistrationControllerBase> logger, TestRegistrationTokenStore tokens)
            : base(env, logger)
        {
            _tokens = tokens;
        }

        // Shows a platform accepting its own opaque, single-use tokens, and falling back to the default
        // JWT check for anything else.
        protected override Task<ActionResult> AuthorizeRegistrationAsync(string registrationToken)
        {
            return _tokens.TryRedeem(registrationToken)
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

    /// <summary>
    /// Opaque, single-use registration tokens issued by the test platform, as a real platform
    /// would keep them in a database.
    /// </summary>
    public class TestRegistrationTokenStore
    {
        private readonly ConcurrentDictionary<string, bool> _issued = new ConcurrentDictionary<string, bool>();

        public string Issue()
        {
            var token = Guid.NewGuid().ToString("N");
            _issued[token] = true;
            return token;
        }

        public bool TryRedeem(string token) => token != null && _issued.TryRemove(token, out _);
    }
}
