using System.Diagnostics;

namespace LtiAdvantage.IdentityServer4
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
