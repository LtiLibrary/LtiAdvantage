using System.Diagnostics;

namespace LtiAdvantage.IdentityServer4
{
    /// <summary>
    /// Local version of Identity Server 4 internal static class StringExtensions
    /// https://github.com/IdentityServer/IdentityServer4/blob/master/src/Extensions/StringsExtensions.cs
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Ensure the string has a trailing slash.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
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
        /// True if the value is null, empty, or whitespace.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// True if the value is not null, empty, or whitespace.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
