using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Server;

namespace LtiAdvantage.OpenIddict.Validation
{
    /// <summary>
    /// Replace the subject in the authorize request, with a <see cref="T:System.Security.Claims.ClaimsPrincipal" />
    /// for the person being impersonated. For example a student in a course.
    /// </summary>
    public class ImpersonationAuthorizeRequestValidator(ILogger<ImpersonationAuthorizeRequestValidator> logger)
        : IOpenIddictServerHandler<OpenIddictServerEvents.ProcessSignInContext>
    {
        public const string AuthenticationType = @"Impersonation";

        public ValueTask HandleAsync(OpenIddictServerEvents.ProcessSignInContext context)
        {
            // Continue with the next validator if there is no principal
            if (context.Principal == null)
            {
                context.HandleRequest();
                return ValueTask.CompletedTask;
            }

            var subject = context.Principal.FindFirstValue(OpenIddictConstants.Claims.Subject);
            var loginHint = context.Request.LoginHint;

            if (!string.IsNullOrEmpty(loginHint) && subject != loginHint)
            {
                logger.LogInformation($"Impersonating subject {loginHint}.");

                // Replace the subject with the person being impersonated in login_hint
                var identity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(OpenIddictConstants.Claims.Subject, loginHint),
                    new Claim("auth_time", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                    new Claim("idp", "local")
                }, AuthenticationType);

                context.Principal = new ClaimsPrincipal(identity);
            }

            context.HandleRequest();
            return ValueTask.CompletedTask;
        }
    }
}