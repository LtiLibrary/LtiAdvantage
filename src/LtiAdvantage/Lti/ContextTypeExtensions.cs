using System;
using System.Collections.Generic;
using System.Linq;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    /// <summary>
    /// Helpers for <see cref="ContextType"/> values.
    /// </summary>
    public static class ContextTypeExtensions
    {
        private static readonly ContextType[] Known = Enum.GetValues(typeof(ContextType)).Cast<ContextType>()
            .Where(type => type != ContextType.Unknown)
            .ToArray();

        private static readonly Dictionary<string, ContextType> ByUri = Known
            .SelectMany(type => (((Enum) type).GetUris() ?? Array.Empty<string>()).Select(uri => (uri, type)))
            .GroupBy(pair => pair.uri, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First().type, StringComparer.Ordinal);

        private static readonly Dictionary<ContextType, string> UriByType = Known
            .Select(type => (type, uri: (((Enum) type).GetUris() ?? Array.Empty<string>())
                .FirstOrDefault(u => u.StartsWith("http", StringComparison.Ordinal))))
            .Where(pair => pair.uri != null)
            .ToDictionary(pair => pair.type, pair => pair.uri);

        /// <summary>
        /// Returns the context type's full URI, as the context claim writes it
        /// (<c>http://purl.imsglobal.org/vocab/lis/v2/course#CourseOffering</c>), or null for
        /// <see cref="ContextType.Unknown"/>.
        /// </summary>
        public static string ToUri(this ContextType contextType) =>
            UriByType.TryGetValue(contextType, out var uri) ? uri : null;

        /// <summary>
        /// Parses a context type from its URI, the simple name the spec allows (such as
        /// <c>CourseOffering</c>), or the enum name in any case.
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
            if (ByUri.TryGetValue(value, out contextType))
            {
                return true;
            }

            foreach (var type in Known)
            {
                if (string.Equals(type.ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    contextType = type;
                    return true;
                }
            }

            contextType = ContextType.Unknown;
            return false;
        }
    }
}
