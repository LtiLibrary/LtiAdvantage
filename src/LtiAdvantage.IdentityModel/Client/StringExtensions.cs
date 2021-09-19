using System.Diagnostics;

namespace LtiAdvantage.Utilities
{
    /// <summary>
    /// Local version of Identity Server 4 internal static class StringExtensions
    /// https://github.com/IdentityServer/IdentityServer4/blob/master/src/Extensions/StringsExtensions.cs
    /// </summary>
    public static class StringExtensions
    {
        [DebuggerStepThrough]
        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
            {
                return url + "/";
            }

            return url;
        }

        /// <summary>
        /// Returns "[Not Set]" or replacement if string is missing.
        /// </summary>
        /// <param name="value">The string.</param>
        /// <param name="replacement">The replacement (defaults to "[Not Set]").</param>
        /// <returns>A string.</returns>
        [DebuggerStepThrough]
        public static string IfMissingThen(this string value, string replacement = "[Not Set]")
        {
            return string.IsNullOrWhiteSpace(value) ? replacement : value;
        }

        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
