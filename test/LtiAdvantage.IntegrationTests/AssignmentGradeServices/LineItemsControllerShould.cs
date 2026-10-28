using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.AssignmentGradeServices
{
    public class LineItemsControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly IHost _host;

        private const string LineItemsUrl = "/context/1234/lineitems";
        private const string LineItemUrl = "/context/1234/lineitems/1234";

        public LineItemsControllerShould()
        {
            _host = TestHostFactory.Create();
            _client = _host.GetTestClient();
        }

        /// <summary>
        /// Check the return status code and content-type.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Ags.LineItem, HttpStatusCode.OK, Constants.MediaTypes.LineItem)]
        [InlineData(Constants.LtiScopes.Ags.LineItemReadonly, HttpStatusCode.Forbidden, "")]
        public async Task AddLineItem_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
#if NET7_0_OR_GREATER
            var lineItemContent = new StringContent(JsonSerializer.Serialize(new LineItem()),
                Encoding.UTF8, new MediaTypeHeaderValue(Constants.MediaTypes.LineItem));
#else
            var lineItemContent = new StringContent(JsonSerializer.Serialize(new LineItem()),
                Encoding.UTF8, Constants.MediaTypes.LineItem);
#endif

            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.PostAsync(LineItemsUrl, lineItemContent);
            Assert.Equal(statusCode, response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                Assert.StartsWith(contentType, response.Content.Headers.ContentType.ToString());
            }
        }

        /// <summary>
        /// Check the return status code.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Ags.LineItem, HttpStatusCode.OK)]
        [InlineData(Constants.LtiScopes.Ags.LineItemReadonly, HttpStatusCode.Forbidden)]
        public async Task DeleteLineItem_WhenScopeAllows(string scope, HttpStatusCode statusCode)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.DeleteAsync(LineItemUrl);
            Assert.Equal(statusCode, response.StatusCode);
        }

        /// <summary>
        /// Check the return status code and content-type.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Ags.LineItem, HttpStatusCode.OK, Constants.MediaTypes.LineItem)]
        [InlineData(Constants.LtiScopes.Ags.LineItemReadonly, HttpStatusCode.OK, Constants.MediaTypes.LineItem)]
        [InlineData(Constants.LtiScopes.Nrps.MembershipReadonly, HttpStatusCode.Forbidden, "")]
        public async Task ReturnLineItem_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.GetAsync(LineItemUrl);
            Assert.Equal(statusCode, response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                Assert.StartsWith(contentType, response.Content.Headers.ContentType.ToString());
            }
        }

        /// <summary>
        /// Check the return status code and content-type.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Ags.LineItem, HttpStatusCode.OK, Constants.MediaTypes.LineItemContainer)]
        [InlineData(Constants.LtiScopes.Ags.LineItemReadonly, HttpStatusCode.OK, Constants.MediaTypes.LineItemContainer)]
        [InlineData(Constants.LtiScopes.Nrps.MembershipReadonly, HttpStatusCode.Forbidden, "")]
        public async Task ReturnLineItems_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.GetAsync(LineItemsUrl);
            Assert.Equal(statusCode, response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                Assert.StartsWith(contentType, response.Content.Headers.ContentType.ToString());
            }
        }

        /// <summary>
        /// Check the return status code.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Ags.LineItem, HttpStatusCode.OK)]
        [InlineData(Constants.LtiScopes.Ags.LineItemReadonly, HttpStatusCode.Forbidden)]
        public async Task UpdateLineItem_WhenScopeAllows(string scope, HttpStatusCode statusCode)
        {
            var lineItemContent = new StringContent(JsonSerializer.Serialize(new LineItem()),
                Encoding.UTF8, Constants.MediaTypes.LineItem);

            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.PutAsync(LineItemUrl, lineItemContent);
            Assert.Equal(statusCode, response.StatusCode);
        }

        /// <summary>AGS 2.0 names the line item filters in snake_case.</summary>
        [Fact]
        public async Task BindTheSpecFilterNames()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Ags.LineItemReadonly);

            var response = await _client.GetAsync(LineItemsUrl + "?resource_link_id=rl-1&resource_id=r-1&tag=final");

            response.EnsureSuccessStatusCode();
            Assert.Equal("rl-1", Header(response, "x-resource-link-id"));
            Assert.Equal("r-1", Header(response, "x-resource-id"));
            Assert.Equal("final", Header(response, "x-tag"));
        }

        /// <summary>The camelCase names 4.0.0 bound still work, so existing callers keep their filters.</summary>
        [Fact]
        public async Task StillBindTheOldFilterNames()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Ags.LineItemReadonly);

            var response = await _client.GetAsync(LineItemsUrl + "?resourceLinkId=rl-1&resourceId=r-1");

            response.EnsureSuccessStatusCode();
            Assert.Equal("rl-1", Header(response, "x-resource-link-id"));
            Assert.Equal("r-1", Header(response, "x-resource-id"));
        }

        [Fact]
        public async Task PreferTheSpecFilterName_WhenBothAreSent()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Ags.LineItemReadonly);

            var response = await _client.GetAsync(LineItemsUrl + "?resource_link_id=spec&resourceLinkId=old");

            response.EnsureSuccessStatusCode();
            Assert.Equal("spec", Header(response, "x-resource-link-id"));
        }

        [Theory]
        [InlineData("?resource_link_id=&resourceLinkId=old", "")]
        [InlineData("?resourceLinkId=a&resourceLinkId=b", "a")]
        public async Task ReadTheOldFilterName_OnlyWithoutTheSpecName_AndOnlyItsFirstValue(string query, string expected)
        {
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.Ags.LineItemReadonly);

            var response = await _client.GetAsync(LineItemsUrl + query);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, Header(response, "x-resource-link-id"));
        }

        private static string Header(HttpResponseMessage response, string name) =>
            string.Join(",", response.Headers.GetValues(name));

        public void Dispose()
        {
            _client?.Dispose();
            _host?.Dispose();
        }
    }
}
