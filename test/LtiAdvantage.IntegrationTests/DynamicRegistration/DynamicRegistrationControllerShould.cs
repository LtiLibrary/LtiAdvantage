using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.DynamicRegistration
{
    public class DynamicRegistrationControllerShould : IDisposable
    {
        private const string Url = "lti/register";
        private readonly HttpClient _client;
        private readonly TestServer _server;

        public DynamicRegistrationControllerShould()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureLogging(l => { l.AddConsole(); l.AddDebug(); })
                .UseStartup<Startup>());
            _client = _server.CreateClient();
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

        public void Dispose() { _client?.Dispose(); _server?.Dispose(); }
    }
}
