using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace LtiAdvantage.AspNetCore.Utilities
{
    /// <summary>
    /// Moves one of the library's named routes, such as
    /// <see cref="Constants.ServiceEndpoints.OpenIdConnect.OpenIdConfigurationService"/>, to a
    /// different path. Add it with <c>MvcOptions.MapLtiRoute</c>.
    /// </summary>
    /// <remarks>
    /// The controller bases put fixed routes on their actions, for example
    /// <c>.well-known/openid-configuration</c>. An app that already serves that path, such as an
    /// OpenIddict server with its own discovery endpoint, can move the LTI one instead of turning
    /// the other off. Only the route with this name changes; unnamed alternate routes on the same
    /// action (the <c>.{format}</c> variants of the service routes) keep their template.
    /// </remarks>
    public class LtiRouteConvention : IApplicationModelConvention
    {
        /// <summary>
        /// Creates a convention that gives the route named <paramref name="routeName"/> the
        /// template <paramref name="template"/>.
        /// </summary>
        /// <param name="routeName">The route name, one of <see cref="Constants.ServiceEndpoints"/>.</param>
        /// <param name="template">The new route template, for example <c>lti/openid-configuration</c>.</param>
        public LtiRouteConvention(string routeName, string template)
        {
            if (string.IsNullOrWhiteSpace(routeName)) throw new ArgumentNullException(nameof(routeName));
            if (string.IsNullOrWhiteSpace(template)) throw new ArgumentNullException(nameof(template));
            RouteName = routeName;
            Template = template;
        }

        /// <summary>The name of the route to move.</summary>
        public string RouteName { get; }

        /// <summary>The new route template.</summary>
        public string Template { get; }

        /// <inheritdoc />
        public void Apply(ApplicationModel application)
        {
            var routes = application.Controllers
                .SelectMany(c => c.Actions)
                .SelectMany(a => a.Selectors)
                .Select(s => s.AttributeRouteModel)
                .Where(r => r != null && string.Equals(r.Name, RouteName, StringComparison.Ordinal))
                .ToList();

            if (routes.Count == 0)
            {
                // A typo in the name would otherwise leave the route where it was without a word.
                throw new InvalidOperationException(
                    $"No route named '{RouteName}' was found. Use one of the Constants.ServiceEndpoints names " +
                    "of a controller this app maps.");
            }

            foreach (var route in routes)
            {
                route.Template = Template;
            }
        }
    }
}
