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

        /// <summary>
        /// Adds support for client authentication using JWT bearer assertions signed
        /// with client private key stored in PEM format rather than X509Certificate2 format.
        /// </summary>
        public static OpenIddictServerBuilder AddLtiJwtBearerClientAuthentication(this OpenIddictServerBuilder builder)
        {
            builder.Services.AddLogging();
            builder.Services.AddSingleton<PrivatePemKeyJwtSecretValidator>();
            builder.AddEventHandler<OpenIddict.Server.OpenIddictServerEvents.ValidateTokenRequestContext>(
                provider => provider.UseScopedHandler<PrivatePemKeyJwtSecretValidator>()
            );

            return builder;
        }
    }
}