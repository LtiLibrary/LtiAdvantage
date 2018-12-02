using System;
using LtiAdvantage.IdentityServer4.ResponseHandling;
using LtiAdvantage.IdentityServer4.Validation;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Builder extension methods for registering additional services.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds impersonation support to authorization.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IIdentityServerBuilder  AddImpersonationSupport(this IIdentityServerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddCustomAuthorizeRequestValidator<ImpersonationAuthorizeRequestValidator>();
            builder.AddAuthorizeInteractionResponseGenerator<ImpersonationInteractionResponseGenerator>();

            return builder;
        }
    }
}
