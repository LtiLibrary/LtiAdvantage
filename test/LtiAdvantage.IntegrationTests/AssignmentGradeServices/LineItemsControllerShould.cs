using System;
using System.Net;
using System.Net.Http;
using System.Text;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;

namespace LtiAdvantage.IntegrationTests.AssignmentGradeServices
{
    public class LineItemsControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        private const string LineItemsUrl = "/context/1234/lineitems";
        private const string LineItemUrl = "/context/1234/lineitems/1234";

        public LineItemsControllerShould()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                    logging.AddDebug();
                })
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        /// <summary>
        /// Check the return status code and content-type.
        /// </summary>
        [Theory]
        [InlineData(Constants.LtiScopes.Ags.LineItem, HttpStatusCode.OK, Constants.MediaTypes.LineItem)]
        [InlineData(Constants.LtiScopes.Ags.LineItemReadonly, HttpStatusCode.Forbidden, "")]
        public async void AddLineItem_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
        {
            var lineItemContent = new StringContent(JsonConvert.SerializeObject(new LineItem()),
                Encoding.UTF8, Constants.MediaTypes.LineItem);

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
        public async void DeleteLineItem_WhenScopeAllows(string scope, HttpStatusCode statusCode)
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
        public async void ReturnLineItem_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
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
        public async void ReturnLineItems_WhenScopeAllows(string scope, HttpStatusCode statusCode, string contentType)
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
        public async void UpdateLineItem_WhenScopeAllows(string scope, HttpStatusCode statusCode)
        {
            var lineItemContent = new StringContent(JsonConvert.SerializeObject(new LineItem()),
                Encoding.UTF8, Constants.MediaTypes.LineItem);

            _client.DefaultRequestHeaders.Add("x-test-scope", scope);
            var response = await _client.PutAsync(LineItemUrl, lineItemContent);
            Assert.Equal(statusCode, response.StatusCode);
        }

        public void Dispose()
        {
            _client?.Dispose();
            _server?.Dispose();
        }
    }
}
