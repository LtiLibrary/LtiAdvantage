using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IdentityServer4.ResponseHandling
{
    /// <inheritdoc />
    /// <summary>
    /// Prevents impersonating user from having to login during authorization request.
    /// </summary>
    public class ImpersonationInteractionResponseGenerator : AuthorizeInteractionResponseGenerator
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="ImpersonationInteractionResponseGenerator"/> class.
        /// </summary>
        /// <param name="clock">The clock.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="consent">The consent.</param>
        /// <param name="profile">The profile.</param>
        public ImpersonationInteractionResponseGenerator(
            ISystemClock clock, 
            ILogger<AuthorizeInteractionResponseGenerator> logger, 
            IConsentService consent, 
            IProfileService profile) : base(clock, logger, consent, profile)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Do not force impersonating user to login during authorization request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="consent"></param>
        /// <returns></returns>
        public override Task<InteractionResponse> ProcessInteractionAsync(ValidatedAuthorizeRequest request, ConsentResponse consent = null)
        {
            if (!request.Subject.IsAuthenticated())
            {
                var subject = request.Subject.Claims.SingleOrDefault(c => c.Type == "sub")?.Value;
                var loginHint = request.LoginHint;

                if (subject == loginHint)
                {
                    return Task.FromResult(new InteractionResponse {IsLogin = false});
                }
            }

            return base.ProcessInteractionAsync(request, consent);
        }
    }
}
