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
    /// the other off. The action's <c>.{format}</c> variant, which the service routes have, moves
    /// with it. The new template must keep the route's parameters (such as <c>{contextId}</c>), or
    /// the action no longer receives them.
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
            if (string.IsNullOrWhiteSpace(routeName)) throw new ArgumentException("A route name is required.", nameof(routeName));
            if (string.IsNullOrWhiteSpace(template)) throw new ArgumentException("A route template is required.", nameof(template));
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
            var moved = 0;
            foreach (var action in application.Controllers.SelectMany(c => c.Actions))
            {
                var named = action.Selectors
                    .Select(s => s.AttributeRouteModel)
                    .FirstOrDefault(r => r != null && string.Equals(r.Name, RouteName, StringComparison.Ordinal));
                if (named == null)
                {
                    continue;
                }

                // The service routes also answer at "<template>.{format}"; move that alias too, or
                // the old path keeps working.
                var formatAlias = named.Template + ".{format}";
                foreach (var route in action.Selectors.Select(s => s.AttributeRouteModel))
                {
                    if (route != null && route.Name == null && string.Equals(route.Template, formatAlias, StringComparison.Ordinal))
                    {
                        route.Template = Template + ".{format}";
                    }
                }

                named.Template = Template;
                moved++;
            }

            if (moved == 0)
            {
                // A typo in the name would otherwise leave the route where it was without a word.
                throw new InvalidOperationException(
                    $"No route named '{RouteName}' was found. Use one of the Constants.ServiceEndpoints names " +
                    "of a controller this app maps.");
            }
        }
    }
}
