using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LtiAdvantage.AspNetCore.Utilities
{
    /// <inheritdoc />
    /// <summary>
    /// Dynamically add required scope authorization policy.
    /// See https://www.jerriepelser.com/blog/creating-dynamic-authorization-policies-aspnet-core/.
    /// </summary>
    /// <example>
    /// To protect an API:
    /// 
    /// [Authorize(Policy = "scope")]
    /// </example>
    /// <example>
    /// If either scope is sufficient, separate with a space:
    /// 
    /// [Authorize("scope1 scope2")]
    /// </example>
    public class AuthorizationScopePolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly ILogger<AuthorizationScopePolicyProvider> _logger;
        private readonly IOptions<AuthorizationOptions> _options;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public AuthorizationScopePolicyProvider(
            ILogger<AuthorizationScopePolicyProvider> logger,
            IOptions<AuthorizationOptions> options) : base(options)
        {
            _logger = logger;
            _options = options;
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns a policy that requires a scope claim = policyName.
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns></returns>
        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (string.IsNullOrWhiteSpace(policyName))
            {
                _logger.LogError($"{nameof(policyName)} is required.");
                return null;
            }

            // Check static policies first
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                _logger.LogInformation($"Adding required scope {policyName}.");

                var requiredScopes = policyName.Split(' ');
                policy = new AuthorizationPolicyBuilder().AddRequirements()
                    .RequireAssertion(context =>
                    {
                        // Get the scope claim from the user's principal
                        var scopeClaim = context.User.FindFirst("scope");

                        // If there's no scope claim, the assertion fails
                        if (scopeClaim == null)
                        {
                            return false;
                        }

                        // Split the scope claim value into individual scopes
                        var userScopes = scopeClaim.Value.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

                        // Check if any of the required scopes are present in the user's scopes
                        return requiredScopes.Any(requiredScope => userScopes.Contains(requiredScope));
                    })
                    .Build();

                // Add policy to the AuthorizationOptions, so we don't have to re-create it each time
                _options.Value.AddPolicy(policyName, policy);
            }

            return policy;
        }
    }
}
