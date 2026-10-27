using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.DynamicRegistration
{
    public class DynamicRegistrationControllerShould : IDisposable
    {
        private const string Url = "lti/register";
        private readonly HttpClient _client;
        private readonly IHost _host;

        public DynamicRegistrationControllerShould()
        {
            _host = TestHostFactory.Create();
            _client = _host.GetTestClient();
        }

        [Fact]
        public async Task Reject_WhenScopeMissing()
        {
            // Authenticated but with a non-registration scope → policy denies.
            _client.DefaultRequestHeaders.Add("x-test-scope",
                Constants.LtiScopes.Nrps.MembershipReadonly);
            var resp = await PostAsync(MinimalTool());
            Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
        }

        [Fact]
        public async Task RegisterTool_AndReturnClientId()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope",
                Constants.LtiScopes.DynamicRegistration.Scope);

            var resp = await PostAsync(MinimalTool());

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var body = await resp.Content.ReadAsStringAsync();
            var registered = JsonSerializer.Deserialize<ToolConfiguration>(body);
            Assert.False(string.IsNullOrWhiteSpace(registered.ClientId));
            Assert.Equal("Example Tool", registered.ClientName);
        }

        [Fact]
        public async Task Reject_InvalidClientMetadata_With400()
        {
            _client.DefaultRequestHeaders.Add("x-test-scope",
                Constants.LtiScopes.DynamicRegistration.Scope);

            var resp = await PostAsync(new ToolConfiguration { ClientName = "Example Tool" });

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("invalid_client_metadata", doc.RootElement.GetProperty("error").GetString());
            var description = doc.RootElement.GetProperty("error_description").GetString();
            Assert.Contains("initiate_login_uri", description);
            Assert.Contains("redirect_uris", description);
            Assert.Contains("jwks_uri", description);
        }

        [Fact]
        public async Task Reject_WhenUnauthenticated()
        {
            var resp = await PostAsync(MinimalTool());
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task AcceptOpaqueRegistrationToken_WhenOverrideAllowsIt()
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", Controllers.DynamicRegistrationController.OpaqueRegistrationToken);

            var resp = await PostAsync(MinimalTool());

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        }

        [Fact]
        public async Task Answer400_WhenOverrideRejects()
        {
            AuthorizeWithRegistrationScope();
            var tool = MinimalTool();
            tool.ClientName = "reject";

            var resp = await PostAsync(tool);

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("invalid_redirect_uri", doc.RootElement.GetProperty("error").GetString());
            Assert.Equal("Redirect URI not allowed.", doc.RootElement.GetProperty("error_description").GetString());
        }

        [Theory]
        [InlineData("return-null")]
        [InlineData("no-client-id")]
        public async Task Answer500WithoutDetails_WhenOverrideReturnsNoClientId(string behavior)
        {
            AuthorizeWithRegistrationScope();
            var tool = MinimalTool();
            tool.ClientName = behavior;

            var resp = await PostAsync(tool);

            Assert.Equal(HttpStatusCode.InternalServerError, resp.StatusCode);
            // The test host runs as Production, where exception details must not be returned.
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.False(doc.RootElement.TryGetProperty("detail", out var detail) && detail.ValueKind == JsonValueKind.String);
        }

        [Fact]
        public async Task IgnoreClientIdSentByTool()
        {
            AuthorizeWithRegistrationScope();
            var tool = MinimalTool();
            tool.ClientName = "keep-client-id";
            tool.ClientId = "existing-tool-client-id";

            var resp = await PostAsync(tool);

            Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
            var registered = JsonSerializer.Deserialize<ToolConfiguration>(await resp.Content.ReadAsStringAsync());
            Assert.False(string.IsNullOrWhiteSpace(registered.ClientId));
            Assert.NotEqual("existing-tool-client-id", registered.ClientId);
        }

        [Fact]
        public async Task Reject_MissingTokenEndpointAuthMethodAndApplicationType()
        {
            AuthorizeWithRegistrationScope();
            var tool = MinimalTool();
            tool.TokenEndpointAuthMethod = null;
            tool.ApplicationType = null;

            var description = await AssertInvalidClientMetadata(await PostAsync(tool), "token_endpoint_auth_method");
            Assert.Contains("application_type", description);
        }

        [Fact]
        public async Task Answer401_BeforeReadingBody_WhenUnauthenticated()
        {
            var resp = await _client.PostAsync(Url, new StringContent("not json", Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Theory]
        [InlineData("/login")]
        [InlineData("javascript:alert(1)")]
        [InlineData("file:///etc/passwd")]
        [InlineData("ftp://tool.example.com/login")]
        [InlineData("http://tool.example.com/login")]
        public async Task Reject_NonHttpsUrls(string initiateLoginUri)
        {
            AuthorizeWithRegistrationScope();
            var tool = MinimalTool();
            tool.InitiateLoginUri = initiateLoginUri;

            var resp = await PostAsync(tool);

            await AssertInvalidClientMetadata(resp, "initiate_login_uri");
        }

        [Fact]
        public async Task Reject_WrongTokenEndpointAuthMethod()
        {
            AuthorizeWithRegistrationScope();
            var tool = MinimalTool();
            tool.TokenEndpointAuthMethod = "client_secret_basic";

            await AssertInvalidClientMetadata(await PostAsync(tool), "token_endpoint_auth_method");
        }

        [Fact]
        public async Task Reject_MissingGrantAndResponseTypes()
        {
            AuthorizeWithRegistrationScope();
            var tool = MinimalTool();
            tool.GrantTypes = new[] { "authorization_code" };
            tool.ResponseTypes = new[] { "code" };

            var resp = await PostAsync(tool);

            var description = await AssertInvalidClientMetadata(resp, "grant_types");
            Assert.Contains("client_credentials", description);
            Assert.Contains("response_types", description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not json")]
        [InlineData("null")]
        public async Task Reject_MalformedBody_WithRegistrationError(string body)
        {
            AuthorizeWithRegistrationScope();

            var resp = await _client.PostAsync(Url, new StringContent(body, Encoding.UTF8, "application/json"));

            await AssertInvalidClientMetadata(resp, null);
        }

        private void AuthorizeWithRegistrationScope() =>
            _client.DefaultRequestHeaders.Add("x-test-scope", Constants.LtiScopes.DynamicRegistration.Scope);

        private static async Task<string> AssertInvalidClientMetadata(HttpResponseMessage resp, string expectedInDescription)
        {
            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            Assert.Equal("invalid_client_metadata", doc.RootElement.GetProperty("error").GetString());
            var description = doc.RootElement.GetProperty("error_description").GetString();
            Assert.False(string.IsNullOrWhiteSpace(description));
            if (expectedInDescription != null) Assert.Contains(expectedInDescription, description);
            return description;
        }

        private Task<HttpResponseMessage> PostAsync(ToolConfiguration tool)
        {
            var json = JsonSerializer.Serialize(tool);
            return _client.PostAsync(Url, new StringContent(json, Encoding.UTF8, "application/json"));
        }

        private static ToolConfiguration MinimalTool() => new()
        {
            ApplicationType = "web",
            ClientName = "Example Tool",
            InitiateLoginUri = "https://tool.example.com/login",
            RedirectUris = new[] { "https://tool.example.com/launch" },
            JwksUri = "https://tool.example.com/.well-known/jwks.json",
            ResponseTypes = new[] { "id_token" },
            GrantTypes = new[] { "implicit", "client_credentials" },
            TokenEndpointAuthMethod = "private_key_jwt",
            LtiToolConfiguration = new LtiToolConfiguration
            {
                Domain = "tool.example.com",
                TargetLinkUri = "https://tool.example.com/launch"
            }
        };

        public void Dispose() { _client?.Dispose(); _host?.Dispose(); }
    }
}
