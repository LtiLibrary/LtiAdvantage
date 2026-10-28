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

        // Parameters run to the next comma outside quotes (RFC 8288 allows title="a, b").
        private static readonly Regex LinkValue = new Regex(
            "<(?<url>[^>]*)>(?<params>(?:[^,\"]|\"[^\"]*\")*)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

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
            return GetNextPageUrl(response, response.RequestMessage?.RequestUri);
        }

        /// <summary>
        /// Returns the <c>rel="next"</c> URL from the response's <c>Link</c> header, resolved against
        /// <paramref name="baseUrl"/>, or null when there is no next page.
        /// </summary>
        public static Uri GetNextPageUrl(this HttpResponseMessage response, Uri baseUrl)
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
                // Only http(s) counts as absolute: on Unix "/path" would otherwise parse as a file URI.
                if (Uri.TryCreate(url, UriKind.Absolute, out var absolute)
                    && (absolute.Scheme == Uri.UriSchemeHttps || absolute.Scheme == Uri.UriSchemeHttp))
                {
                    return absolute;
                }

                if (baseUrl != null && Uri.TryCreate(baseUrl, url, out var resolved))
                {
                    return resolved;
                }

                // Unresolvable: another next link may still work.
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

            var body = await ReadBoundedAsync(response.Content, MaxBodyLength, truncate: true, cancellationToken)
                .ConfigureAwait(false) ?? string.Empty;

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

        /// <summary>
        /// Reads at most <paramref name="maxChars"/> characters of the body without buffering more.
        /// Longer bodies are cut (<paramref name="truncate"/>) or rejected.
        /// </summary>
        internal static async Task<string> ReadBoundedAsync(HttpContent content, int maxChars, bool truncate,
            CancellationToken cancellationToken)
        {
            if (content == null)
            {
                return null;
            }

            using var stream = await content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            using var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true);
            var text = new System.Text.StringBuilder();
            var chunk = new char[8192];
            int read;
            while ((read = await reader.ReadAsync(chunk.AsMemory(), cancellationToken).ConfigureAwait(false)) > 0)
            {
                if (text.Length + read > maxChars)
                {
                    if (!truncate)
                        throw new InvalidOperationException($"The platform response is larger than {maxChars} characters.");
                    text.Append(chunk, 0, maxChars - text.Length);
                    break;
                }

                text.Append(chunk, 0, read);
            }

            return text.ToString();
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
