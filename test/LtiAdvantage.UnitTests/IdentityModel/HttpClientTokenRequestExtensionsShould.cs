using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LtiAdvantage.IdentityModel.Client;
using Xunit;

namespace LtiAdvantage.UnitTests.IdentityModel
{
    public class HttpClientTokenRequestExtensionsShould
    {
        [Fact]
        public async Task SendClientAssertionInTheRequestBody()
        {
            var handler = new CapturingHandler();
            using var client = new HttpClient(handler);

            var response = await client.RequestClientCredentialsTokenWithJwtAsync(new JwtClientCredentialsTokenRequest
            {
                Address = "https://platform.example.com/connect/token",
                ClientId = "tool",
                Jwt = "header.payload.signature",
                Scope = "scope1 scope2"
            });

            Assert.False(response.IsError, response.Error);
            Assert.Null(handler.Request.Headers.Authorization);
            Assert.Contains("grant_type=client_credentials", handler.Body);
            Assert.Contains("client_assertion=header.payload.signature", handler.Body);
            Assert.Contains("client_assertion_type=urn%3Aietf%3Aparams%3Aoauth%3Aclient-assertion-type%3Ajwt-bearer", handler.Body);
            Assert.Contains("scope=scope1+scope2", handler.Body);
        }

        private class CapturingHandler : HttpMessageHandler
        {
            public HttpRequestMessage Request { get; private set; }
            public string Body { get; private set; }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Request = request;
                Body = await request.Content!.ReadAsStringAsync(cancellationToken);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"access_token\":\"token\",\"token_type\":\"Bearer\",\"expires_in\":3600}",
                        Encoding.UTF8, "application/json")
                };
            }
        }
    }
}
