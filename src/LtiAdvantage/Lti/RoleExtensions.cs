using System;
using System.Collections.Generic;
using System.Linq;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    /// <summary>
    /// Helpers for <see cref="Role"/> values.
    /// </summary>
    public static class RoleExtensions
    {
        private static readonly Dictionary<string, Role> ByUri = Enum.GetValues(typeof(Role)).Cast<Role>()
            .SelectMany(role => (((Enum) role).GetUris() ?? Array.Empty<string>()).Select(uri => (uri, role)))
            .ToDictionary(pair => pair.uri, pair => pair.role, StringComparer.Ordinal);

        private static readonly Dictionary<Role, string> UriByRole = Enum.GetValues(typeof(Role)).Cast<Role>()
            .Select(role => (role, uri: (((Enum) role).GetUris() ?? Array.Empty<string>())
                .FirstOrDefault(u => u.StartsWith("http", StringComparison.Ordinal))))
            .Where(pair => pair.uri != null)
            .ToDictionary(pair => pair.role, pair => pair.uri);

        /// <summary>
        /// Returns the role's full URI, as LTI messages and services write it
        /// (<c>http://purl.imsglobal.org/vocab/lis/v2/membership#Learner</c> for
        /// <see cref="Role.ContextLearner"/>), or null for <see cref="Role.Unknown"/>.
        /// </summary>
        public static string ToUri(this Role role) => UriByRole.TryGetValue(role, out var uri) ? uri : null;

        /// <summary>
        /// Parses a role the way LTI messages and services spell it: the full role URI
        /// (<c>http://purl.imsglobal.org/vocab/lis/v2/membership#Learner</c>), the simple name
        /// the specs allow for context roles (<c>Learner</c>), or the enum name (<c>ContextLearner</c>, any case).
        /// Role URIs and simple names are matched exactly.
        /// </summary>
        /// <param name="value">The role to parse.</param>
        /// <param name="role">The parsed role, or <see cref="Role.Unknown"/> when parsing fails.</param>
        /// <returns>True when <paramref name="value"/> names a known role.</returns>
        public static bool TryParse(string value, out Role role)
        {
            role = Role.Unknown;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            value = value.Trim();
            if (ByUri.TryGetValue(value, out role))
            {
                return true;
            }

            // Enum names only (ignoring case, as ASP.NET Core's enum binding does). Enum.TryParse
            // would also accept numbers ("3", "+3") and lists ("A, B"), so require a plain name.
            if (char.IsLetter(value[0]) && value.All(char.IsLetterOrDigit)
                && Enum.TryParse(value, ignoreCase: true, out role)
                && role != Role.Unknown && Enum.IsDefined(typeof(Role), role))
            {
                return true;
            }

            role = Role.Unknown;
            return false;
        }
    }
}
