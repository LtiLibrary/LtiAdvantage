using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.CourseGroupsService;
using LtiAdvantage.Lti;
using LtiAdvantage.NamesRoleProvisioningService;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.IdentityModel.Client
{
    /// <summary>
    /// HttpClient extensions for calling a platform's LTI services as a tool: Assignment and Grade
    /// Services 2.0, Names and Role Provisioning Services 2.0 and Course Groups Service 1.0.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each call sends <paramref name="accessToken" /> as a bearer token on that request only, so
    /// one <see cref="HttpClient"/> (for example from <c>IHttpClientFactory</c>) can serve several
    /// platforms. Get the token with <c>RequestClientCredentialsTokenWithJwtAsync</c>.
    /// </para>
    /// <para>
    /// List calls follow the <c>Link: rel="next"</c> header and return every page. A next link on
    /// a different origin than the first request is refused, so the token is never sent to another
    /// host. More than <see cref="MaxPages"/> pages, or a link back to a page already read, throws
    /// <see cref="InvalidOperationException"/>.
    /// </para>
    /// <para>
    /// An error status throws <see cref="LtiServiceException"/> with what the platform said.
    /// </para>
    /// </remarks>
    public static class HttpClientLtiServiceExtensions
    {
        /// <summary>The most pages a list call reads before it gives up.</summary>
        public const int MaxPages = 1000;

        private static readonly JsonSerializerOptions WriteOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private static readonly JsonSerializerOptions ReadOptions = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString
        };

        #region Assignment and Grade Services

        /// <summary>
        /// Gets every line item in the context, following paging.
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="accessToken">The access token (scope <c>lineitem</c> or <c>lineitem.readonly</c>).</param>
        /// <param name="lineItemsUrl">The <c>lineitems</c> URL from the launch's AGS endpoint claim.</param>
        /// <param name="resourceLinkId">Optional <c>resource_link_id</c> filter.</param>
        /// <param name="resourceId">Optional <c>resource_id</c> filter.</param>
        /// <param name="tag">Optional <c>tag</c> filter.</param>
        /// <param name="limit">Optional page size hint.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static async Task<IList<LineItem>> GetLineItemsAsync(this HttpClient client, string accessToken,
            string lineItemsUrl, string resourceLinkId = null, string resourceId = null, string tag = null,
            int? limit = null, CancellationToken cancellationToken = default)
        {
            var url = WithQuery(lineItemsUrl, ("resource_link_id", resourceLinkId), ("resource_id", resourceId),
                ("tag", tag), ("limit", Format(limit)));
            var pages = await GetPagesAsync<List<LineItem>>(client, accessToken, url,
                Constants.MediaTypes.LineItemContainer, cancellationToken).ConfigureAwait(false);
            return pages.SelectMany(p => p ?? new List<LineItem>()).ToList();
        }

        /// <summary>Gets one line item.</summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="accessToken">The access token (scope <c>lineitem</c> or <c>lineitem.readonly</c>).</param>
        /// <param name="lineItemUrl">The line item's URL (its <c>id</c>).</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static Task<LineItem> GetLineItemAsync(this HttpClient client, string accessToken, string lineItemUrl,
            CancellationToken cancellationToken = default) =>
            SendAsync<LineItem>(client, accessToken, HttpMethod.Get, lineItemUrl, Constants.MediaTypes.LineItem,
                null, null, cancellationToken);

        /// <summary>Adds a line item to the context and returns it as the platform stored it (with its <c>id</c>).</summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="accessToken">The access token (scope <c>lineitem</c>).</param>
        /// <param name="lineItemsUrl">The <c>lineitems</c> URL from the launch's AGS endpoint claim.</param>
        /// <param name="lineItem">The line item to add.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static Task<LineItem> AddLineItemAsync(this HttpClient client, string accessToken, string lineItemsUrl,
            LineItem lineItem, CancellationToken cancellationToken = default) =>
            SendAsync<LineItem>(client, accessToken, HttpMethod.Post, lineItemsUrl, Constants.MediaTypes.LineItem,
                lineItem ?? throw new ArgumentNullException(nameof(lineItem)), Constants.MediaTypes.LineItem,
                cancellationToken);

        /// <summary>
        /// Replaces a line item. Returns the updated line item, or null when the platform answers
        /// without a body.
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="accessToken">The access token (scope <c>lineitem</c>).</param>
        /// <param name="lineItemUrl">The line item's URL (its <c>id</c>).</param>
        /// <param name="lineItem">The new line item.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static Task<LineItem> UpdateLineItemAsync(this HttpClient client, string accessToken, string lineItemUrl,
            LineItem lineItem, CancellationToken cancellationToken = default) =>
            SendAsync<LineItem>(client, accessToken, HttpMethod.Put, lineItemUrl, Constants.MediaTypes.LineItem,
                lineItem ?? throw new ArgumentNullException(nameof(lineItem)), Constants.MediaTypes.LineItem,
                cancellationToken);

        /// <summary>Deletes a line item.</summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="accessToken">The access token (scope <c>lineitem</c>).</param>
        /// <param name="lineItemUrl">The line item's URL (its <c>id</c>).</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static Task DeleteLineItemAsync(this HttpClient client, string accessToken, string lineItemUrl,
            CancellationToken cancellationToken = default) =>
            SendAsync<object>(client, accessToken, HttpMethod.Delete, lineItemUrl, null, null, null, cancellationToken);

        /// <summary>Posts a score to a line item's <c>scores</c> endpoint.</summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="accessToken">The access token (scope <c>score</c>).</param>
        /// <param name="lineItemUrl">The line item's URL (its <c>id</c>); <c>/scores</c> is added to its path.</param>
        /// <param name="score">The score.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static Task PostScoreAsync(this HttpClient client, string accessToken, string lineItemUrl, Score score,
            CancellationToken cancellationToken = default) =>
            SendAsync<object>(client, accessToken, HttpMethod.Post, AppendPath(lineItemUrl, "scores"), null,
                score ?? throw new ArgumentNullException(nameof(score)), Constants.MediaTypes.Score, cancellationToken);

        /// <summary>Gets every result for a line item, following paging.</summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="accessToken">The access token (scope <c>result.readonly</c>).</param>
        /// <param name="lineItemUrl">The line item's URL (its <c>id</c>); <c>/results</c> is added to its path.</param>
        /// <param name="userId">Optional <c>user_id</c> filter.</param>
        /// <param name="limit">Optional page size hint.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static async Task<IList<Result>> GetResultsAsync(this HttpClient client, string accessToken,
            string lineItemUrl, string userId = null, int? limit = null, CancellationToken cancellationToken = default)
        {
            var url = WithQuery(AppendPath(lineItemUrl, "results"), ("user_id", userId), ("limit", Format(limit)));
            var pages = await GetPagesAsync<List<Result>>(client, accessToken, url,
                Constants.MediaTypes.ResultContainer, cancellationToken).ConfigureAwait(false);
            return pages.SelectMany(p => p ?? new List<Result>()).ToList();
        }

        #endregion

        #region Names and Role Provisioning Services

        /// <summary>
        /// Gets the context's membership, following paging. The returned container holds the members
        /// of every page; its <c>id</c> and <c>context</c> come from the first page.
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="accessToken">The access token (scope <c>contextmembership.readonly</c>).</param>
        /// <param name="membershipUrl">The <c>context_memberships_url</c> from the launch's NRPS claim.</param>
        /// <param name="role">Optional role filter, sent as the role's URI.</param>
        /// <param name="rlid">Optional resource link filter.</param>
        /// <param name="limit">Optional page size hint.</param>
        /// <param name="groups">Ask for each member's group enrollments (Course Groups Service 1.0).</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static async Task<MembershipContainer> GetMembershipAsync(this HttpClient client, string accessToken,
            string membershipUrl, Role? role = null, string rlid = null, int? limit = null, bool groups = false,
            CancellationToken cancellationToken = default)
        {
            var url = WithQuery(membershipUrl, ("role", RoleUri(role)), ("rlid", rlid), ("limit", Format(limit)),
                ("groups", groups ? "true" : null));
            var pages = await GetPagesAsync<MembershipContainer>(client, accessToken, url,
                Constants.MediaTypes.MembershipContainer, cancellationToken).ConfigureAwait(false);

            var result = pages.FirstOrDefault(p => p != null) ?? new MembershipContainer();
            result.Members = pages.Where(p => p?.Members != null).SelectMany(p => p.Members).ToList();
            return result;
        }

        #endregion

        #region Course Groups Service

        /// <summary>
        /// Gets the context's groups, following paging. The returned container holds the groups of
        /// every page.
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="accessToken">The access token (scope <c>contextgroup.readonly</c>).</param>
        /// <param name="contextGroupsUrl">The <c>context_groups_url</c> from the launch's groups service claim.</param>
        /// <param name="userId">Optional <c>user_id</c> filter: only the groups this user is in.</param>
        /// <param name="limit">Optional page size hint.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static async Task<ContextGroupContainer> GetGroupsAsync(this HttpClient client, string accessToken,
            string contextGroupsUrl, string userId = null, int? limit = null, CancellationToken cancellationToken = default)
        {
            var url = WithQuery(contextGroupsUrl, ("user_id", userId), ("limit", Format(limit)));
            var pages = await GetPagesAsync<ContextGroupContainer>(client, accessToken, url,
                Constants.MediaTypes.ContextGroupContainer, cancellationToken).ConfigureAwait(false);

            var result = pages.FirstOrDefault(p => p != null) ?? new ContextGroupContainer();
            result.Groups = pages.Where(p => p?.Groups != null).SelectMany(p => p.Groups).ToList();
            return result;
        }

        /// <summary>
        /// Gets the context's group sets, following paging. The returned container holds the sets of
        /// every page.
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="accessToken">The access token (scope <c>contextgroup.readonly</c>).</param>
        /// <param name="contextGroupSetsUrl">The <c>context_group_sets_url</c> from the launch's groups service claim.</param>
        /// <param name="limit">Optional page size hint.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        public static async Task<ContextGroupSetContainer> GetGroupSetsAsync(this HttpClient client, string accessToken,
            string contextGroupSetsUrl, int? limit = null, CancellationToken cancellationToken = default)
        {
            var url = WithQuery(contextGroupSetsUrl, ("limit", Format(limit)));
            var pages = await GetPagesAsync<ContextGroupSetContainer>(client, accessToken, url,
                Constants.MediaTypes.ContextGroupSetContainer, cancellationToken).ConfigureAwait(false);

            var result = pages.FirstOrDefault(p => p != null) ?? new ContextGroupSetContainer();
            result.Sets = pages.Where(p => p?.Sets != null).SelectMany(p => p.Sets).ToList();
            return result;
        }

        #endregion

        private static async Task<List<T>> GetPagesAsync<T>(HttpClient client, string accessToken, string url,
            string mediaType, CancellationToken cancellationToken)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            var first = ToAbsoluteUri(url, nameof(url));

            var pages = new List<T>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var next = first;
            while (next != null)
            {
                if (!seen.Add(next.AbsoluteUri))
                    throw new InvalidOperationException($"The platform's next page link loops back to {next}.");
                if (pages.Count >= MaxPages)
                    throw new InvalidOperationException($"The platform returned more than {MaxPages} pages.");

                // The token goes with every page, so it must not follow a link to another host.
                if (Uri.Compare(next, first, UriComponents.SchemeAndServer, UriFormat.Unescaped,
                        StringComparison.OrdinalIgnoreCase) != 0)
                    throw new InvalidOperationException(
                        $"The platform's next page link {next} is on a different origin than {first}.");

                using var request = CreateRequest(HttpMethod.Get, next, accessToken, mediaType);
                using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
                await response.EnsureLtiSuccessAsync(cancellationToken).ConfigureAwait(false);
                pages.Add(await ReadAsync<T>(response, cancellationToken).ConfigureAwait(false));
                next = response.GetNextPageUrl();
            }

            return pages;
        }

        private static async Task<T> SendAsync<T>(HttpClient client, string accessToken, HttpMethod method,
            string url, string accept, object body, string contentType, CancellationToken cancellationToken)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            using var request = CreateRequest(method, ToAbsoluteUri(url, nameof(url)), accessToken, accept);
            if (body != null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(body, body.GetType(), WriteOptions),
                    Encoding.UTF8);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }

            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            await response.EnsureLtiSuccessAsync(cancellationToken).ConfigureAwait(false);
            return accept == null ? default : await ReadAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }

        private static HttpRequestMessage CreateRequest(HttpMethod method, Uri url, string accessToken, string accept)
        {
            var request = new HttpRequestMessage(method, url);
            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            if (accept != null)
            {
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
            }

            return request;
        }

        private static async Task<T> ReadAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            var json = response.Content == null
                ? null
                : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            return string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json, ReadOptions);
        }

        private static Uri ToAbsoluteUri(string url, string name)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("A service URL is required.", name);
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
                throw new ArgumentException($"'{url}' is not an absolute http(s) URL.", name);
            return uri;
        }

        /// <summary>Adds a path segment, keeping any query string: <c>…/lineitems/1?a=b</c> → <c>…/lineitems/1/scores?a=b</c>.</summary>
        private static string AppendPath(string url, string segment)
        {
            var builder = new UriBuilder(ToAbsoluteUri(url, nameof(url)));
            builder.Path = builder.Path.TrimEnd('/') + "/" + segment;
            return builder.Uri.AbsoluteUri;
        }

        private static string WithQuery(string url, params (string Name, string Value)[] parameters)
        {
            var builder = new UriBuilder(ToAbsoluteUri(url, nameof(url)));
            var query = builder.Query.TrimStart('?');
            foreach (var (name, value) in parameters.Where(p => p.Value != null))
            {
                query += (query.Length == 0 ? "" : "&") + Uri.EscapeDataString(name) + "=" + Uri.EscapeDataString(value);
            }

            builder.Query = query;
            return builder.Uri.AbsoluteUri;
        }

        private static string Format(int? value) => value?.ToString(CultureInfo.InvariantCulture);

        private static string RoleUri(Role? role)
        {
            if (role == null) return null;
            var uris = ((Enum) role.Value).GetUris();
            return uris?.FirstOrDefault(u => u.StartsWith("http", StringComparison.Ordinal))
                   ?? throw new ArgumentException($"Role {role} has no URI.", nameof(role));
        }
    }
}
