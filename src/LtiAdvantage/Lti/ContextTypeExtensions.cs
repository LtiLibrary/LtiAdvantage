using System;
using System.Linq;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    /// <summary>
    /// Helpers for <see cref="ContextType"/> values.
    /// </summary>
    public static class ContextTypeExtensions
    {
        /// <summary>
        /// Returns the context type's full URI, as the context claim writes it
        /// (<c>http://purl.imsglobal.org/vocab/lis/v2/course#CourseOffering</c>), or null for
        /// <see cref="ContextType.Unknown"/>.
        /// </summary>
        public static string ToUri(this ContextType contextType) =>
            (((Enum) contextType).GetUris() ?? Array.Empty<string>())
            .FirstOrDefault(uri => uri.StartsWith("http", StringComparison.Ordinal));

        /// <summary>
        /// Parses a context type from its URI (or the simple name the spec allows, such as
        /// <c>CourseOffering</c>, where the enum defines one).
        /// </summary>
        /// <param name="value">The value to parse.</param>
        /// <param name="contextType">The parsed type, or <see cref="ContextType.Unknown"/>.</param>
        /// <returns>True when <paramref name="value"/> names a known context type.</returns>
        public static bool TryParse(string value, out ContextType contextType)
        {
            contextType = ContextType.Unknown;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            value = value.Trim();
            foreach (ContextType candidate in Enum.GetValues(typeof(ContextType)))
            {
                if (candidate != ContextType.Unknown
                    && (((Enum) candidate).GetUris() ?? Array.Empty<string>()).Contains(value, StringComparer.Ordinal))
                {
                    contextType = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}
