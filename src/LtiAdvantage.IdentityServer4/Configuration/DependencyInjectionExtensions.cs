using LtiAdvantage.OpenIddict.Validation;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.Abstractions;
using OpenIddict.Server;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for registering additional services.
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds impersonation support to authorization.
        /// </summary>
        /// <remarks>
        /// Allows the application user to impersonate another user type.
        /// </remarks>
        public static OpenIddictServerBuilder AddImpersonationSupport(this OpenIddictServerBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.TryAddSingleton<ImpersonationAuthorizeRequestValidator>();
            builder.AddEventHandler<OpenIddictServerEvents.ProcessSignInContext>(
                provider => provider.UseScopedHandler<ImpersonationAuthorizeRequestValidator>()
            );

            return builder;
        }
    }
}