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

        /// <summary>
        /// Parses a role the way LTI messages and services spell it: the full role URI
        /// (<c>http://purl.imsglobal.org/vocab/lis/v2/membership#Learner</c>), the simple name
        /// the specs allow for context roles (<c>Learner</c>), or the enum name (<c>ContextLearner</c>).
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

            // Enum names only: Enum.TryParse would also accept numbers ("3") and lists ("A, B").
            if (!char.IsDigit(value[0]) && value[0] != '-' && value.IndexOf(',') < 0
                && Enum.TryParse(value, ignoreCase: false, out role)
                && role != Role.Unknown && Enum.IsDefined(typeof(Role), role))
            {
                return true;
            }

            role = Role.Unknown;
            return false;
        }
    }
}
