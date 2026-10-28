using Microsoft.AspNetCore.Http;

namespace LtiAdvantage.AspNetCore.Utilities
{
    /// <summary>
    /// Reads query parameters under the names 4.0.0 bound before they were renamed to the spec names.
    /// </summary>
    internal static class QueryAliasExtensions
    {
        /// <summary>
        /// Returns the first value of <paramref name="legacyName"/>, unless the request also sent
        /// <paramref name="specName"/> (even empty), in which case the spec name wins and this returns null.
        /// </summary>
        public static string LegacyQueryValue(this HttpRequest request, string specName, string legacyName)
        {
            if (request.Query.ContainsKey(specName)
                || !request.Query.TryGetValue(legacyName, out var values)
                || values.Count == 0)
            {
                return null;
            }

            return string.IsNullOrEmpty(values[0]) ? null : values[0];
        }
    }
}
