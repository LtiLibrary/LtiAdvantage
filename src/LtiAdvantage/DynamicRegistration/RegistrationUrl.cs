using System;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// The URL rule shared by the tool and platform sides of Dynamic Registration:
    /// absolute https URLs, with plain http allowed only for loopback hosts (local development).
    /// </summary>
    public static class RegistrationUrl
    {
        /// <summary>
        /// True when <paramref name="uri"/> uses https, or http on a loopback host.
        /// Every other scheme (<c>javascript:</c>, <c>file:</c>, <c>ftp:</c>, ...) is rejected.
        /// </summary>
        public static bool IsAllowed(Uri uri) =>
            uri != null && uri.IsAbsoluteUri && !uri.IsFile && !uri.IsUnc
            && (uri.Scheme == Uri.UriSchemeHttps || (uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback));

        /// <summary>
        /// True when <paramref name="value"/> is an absolute URL that <see cref="IsAllowed(Uri)"/> accepts.
        /// A bare path such as <c>/login</c> is rejected, even though on Unix
        /// <see cref="Uri.TryCreate(string, UriKind, out Uri)"/> parses it as an absolute <c>file:</c> URI.
        /// </summary>
        public static bool IsAllowed(string value) =>
            !string.IsNullOrWhiteSpace(value) && Uri.TryCreate(value, UriKind.Absolute, out var uri) && IsAllowed(uri);
    }
}
