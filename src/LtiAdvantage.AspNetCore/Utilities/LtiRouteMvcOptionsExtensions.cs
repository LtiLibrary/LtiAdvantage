using System;
using LtiAdvantage.AspNetCore.Utilities;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// Extension methods for changing the library's routes.
    /// </summary>
    public static class LtiRouteMvcOptionsExtensions
    {
        /// <summary>
        /// Serves the route named <paramref name="routeName"/> at <paramref name="template"/>
        /// instead of its default path.
        /// </summary>
        /// <example>
        /// Move the LTI discovery document away from OpenIddict's <c>.well-known/openid-configuration</c>:
        /// <code>
        /// services.AddControllers(options => options.MapLtiRoute(
        ///     Constants.ServiceEndpoints.OpenIdConnect.OpenIdConfigurationService,
        ///     "lti/.well-known/openid-configuration"));
        /// </code>
        /// </example>
        /// <param name="options">The MVC options.</param>
        /// <param name="routeName">The route name, one of <see cref="LtiAdvantage.Constants.ServiceEndpoints"/>.</param>
        /// <param name="template">The new route template.</param>
        /// <returns>The options, for chaining.</returns>
        public static MvcOptions MapLtiRoute(this MvcOptions options, string routeName, string template)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.Conventions.Add(new LtiRouteConvention(routeName, template));
            return options;
        }
    }
}
