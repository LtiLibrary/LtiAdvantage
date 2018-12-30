using IdentityServer4.Validation;
using LtiAdvantage.IdentityServer4.Validation;

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
        public static IIdentityServerBuilder AddImpersonationSupport(this IIdentityServerBuilder builder)
        {
            builder.Services.AddLogging();

            builder.AddCustomAuthorizeRequestValidator<ImpersonationAuthorizeRequestValidator>();

            return builder;
        }

        /// <summary>
        /// Adds support for client authentication using JWT bearer assertions signed
        /// with client private key stored in PEM format rather than X509Certificate2 format.
        /// </summary>
        /// <remarks>
        /// See <see cref="IdentityServerBuilderExtensionsAdditional.AddJwtBearerClientAuthentication"/>
        /// for X509Certificate2 version.
        /// </remarks>
        public static IIdentityServerBuilder AddLtiJwtBearerClientAuthentication(this IIdentityServerBuilder builder)
        {
            builder.Services.AddLogging();

            builder.AddSecretParser<JwtBearerClientAssertionSecretParser>();
            builder.AddSecretValidator<PrivatePemKeyJwtSecretValidator>();

            return builder;
        }
    }
}
