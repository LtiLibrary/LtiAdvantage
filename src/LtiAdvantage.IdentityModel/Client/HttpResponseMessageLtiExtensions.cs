using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LtiAdvantage.IdentityModel.Client
{
    /// <summary>
    /// Reads LTI service responses: the RFC 8288 <c>Link</c> header that pages AGS, NRPS and Course
    /// Groups results, and the error body of a failed call.
    /// </summary>
    public static class HttpResponseMessageLtiExtensions
    {
        private const int MaxBodyLength = 4096;

        private static readonly Regex LinkValue = new Regex(
            "<(?<url>[^>]*)>(?<params>[^,]*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex RelParam = new Regex(
            "(?:^|;)\\s*rel\\s*=\\s*(?:\"(?<rel>[^\"]*)\"|(?<rel>[^;\\s]+))",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        /// <summary>
        /// Returns the <c>rel="next"</c> URL from the response's <c>Link</c> header, resolved against
        /// the request URL, or null when there is no next page.
        /// </summary>
        public static Uri GetNextPageUrl(this HttpResponseMessage response)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (!response.Headers.TryGetValues("Link", out var values))
            {
                return null;
            }

            foreach (Match link in values.SelectMany(v => LinkValue.Matches(v)))
            {
                var rel = RelParam.Match(link.Groups["params"].Value);
                if (!rel.Success
                    || !rel.Groups["rel"].Value.Split(' ').Contains("next", StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                var url = link.Groups["url"].Value.Trim();
                var baseUrl = response.RequestMessage?.RequestUri;
                // Only http(s) counts as absolute: on Unix "/path" would otherwise parse as a file URI.
                if (Uri.TryCreate(url, UriKind.Absolute, out var absolute)
                    && (absolute.Scheme == Uri.UriSchemeHttps || absolute.Scheme == Uri.UriSchemeHttp))
                {
                    return absolute;
                }

                return baseUrl != null && Uri.TryCreate(baseUrl, url, out var resolved) ? resolved : null;
            }

            return null;
        }

        /// <summary>
        /// Throws an <see cref="LtiServiceException"/> describing the platform's answer when the
        /// response has an error status. Does nothing for a success status.
        /// </summary>
        public static async Task EnsureLtiSuccessAsync(this HttpResponseMessage response,
            CancellationToken cancellationToken = default)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            var body = response.Content == null
                ? string.Empty
                : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (body.Length > MaxBodyLength)
            {
                body = body.Substring(0, MaxBodyLength);
            }

            string title = null, detail = null, type = null;
            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    using var document = JsonDocument.Parse(body);
                    if (document.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        title = GetString(document.RootElement, "title");
                        detail = GetString(document.RootElement, "detail");
                        type = GetString(document.RootElement, "type");
                    }
                }
                catch (JsonException)
                {
                    // Not JSON (or cut off): the body is reported as text.
                }
            }

            var challenge = response.Headers.WwwAuthenticate.Count == 0
                ? null
                : string.Join(", ", response.Headers.WwwAuthenticate);
            var status = $"{(int) response.StatusCode} {response.ReasonPhrase}".TrimEnd();

            string said;
            if (title != null || detail != null)
                said = string.Join(" · ", new[] {title, detail}.Where(s => !string.IsNullOrWhiteSpace(s)));
            else if (!string.IsNullOrWhiteSpace(body))
                said = body.Length > 300 ? body.Substring(0, 300) + "…" : body;
            else
                said = challenge;

            var message = string.IsNullOrWhiteSpace(said) ? status : $"{status} · {said}";
            throw new LtiServiceException(message, response.StatusCode, title, detail, type, body, challenge);
        }

        private static string GetString(JsonElement element, string name)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase)
                    && property.Value.ValueKind == JsonValueKind.String)
                {
                    return property.Value.GetString();
                }
            }

            return null;
        }
    }
}
