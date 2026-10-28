using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.IdentityModel.Client;
using LtiAdvantage.Lti;
using Xunit;

namespace LtiAdvantage.UnitTests.IdentityModel
{
    public class HttpClientLtiServiceExtensionsShould
    {
        private const string LineItems = "https://platform.example.com/context/1/lineitems";

        [Fact]
        public async Task FollowNextLinks_AndSendTheTokenAndMediaType()
        {
            var handler = new StubHandler(request => request.RequestUri.Query.Contains("page=2")
                ? Json("[{\"id\":\"https://platform.example.com/li/2\",\"label\":\"B\",\"scoreMaximum\":1}]")
                : Json("[{\"id\":\"https://platform.example.com/li/1\",\"label\":\"A\",\"scoreMaximum\":1}]",
                    "<" + LineItems + "?page=2>; rel=\"next\", <" + LineItems + ">; rel=\"first\""));
            var client = new HttpClient(handler);

            var items = await client.GetLineItemsAsync("token-1", LineItems, resourceLinkId: "rl 1", limit: 10);

            Assert.Equal(new[] {"A", "B"}, items.Select(i => i.Label));
            Assert.Equal(2, handler.Requests.Count);
            Assert.All(handler.Requests, r =>
            {
                Assert.Equal("Bearer token-1", r.Headers.Authorization.ToString());
                Assert.Equal(Constants.MediaTypes.LineItemContainer, r.Headers.Accept.Single().MediaType);
            });
            Assert.Equal("?resource_link_id=rl%201&limit=10", handler.Requests[0].RequestUri.Query);
        }

        [Fact]
        public async Task ResolveARelativeNextLink()
        {
            var handler = new StubHandler(request => request.RequestUri.AbsolutePath.EndsWith("/2")
                ? Json("{\"members\":[{\"user_id\":\"u2\"}]}")
                : Json("{\"id\":\"m\",\"members\":[{\"user_id\":\"u1\"}]}", "</context/1/membership/2>; rel=next"));

            var membership = await new HttpClient(handler).GetMembershipAsync("t",
                "https://platform.example.com/context/1/membership", role: Role.ContextLearner);

            Assert.Equal("m", membership.Id);
            Assert.Equal(new[] {"u1", "u2"}, membership.Members.Select(m => m.UserId));
            Assert.Equal("?role=" + Uri.EscapeDataString("http://purl.imsglobal.org/vocab/lis/v2/membership#Learner"),
                handler.Requests[0].RequestUri.Query);
        }

        /// <summary>The token goes with every page, so a next link to another host is refused.</summary>
        [Fact]
        public async Task RefuseANextLinkToAnotherOrigin()
        {
            var handler = new StubHandler(_ => Json("[]", "<https://evil.example.com/steal>; rel=\"next\""));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new HttpClient(handler).GetLineItemsAsync("t", LineItems));
            Assert.Single(handler.Requests);
        }

        [Fact]
        public async Task StopWhenANextLinkLoops()
        {
            var handler = new StubHandler(_ => Json("[]", "<" + LineItems + ">; rel=\"next\""));

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new HttpClient(handler).GetLineItemsAsync("t", LineItems));
        }

        [Fact]
        public async Task PostAScore_ToTheScoresPath_KeepingTheQuery()
        {
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.NoContent));

            await new HttpClient(handler).PostScoreAsync("t", "https://platform.example.com/li/1?x=y", new Score
            {
                UserId = "u1", ActivityProgress = ActivityProgress.Started, GradingProgress = GradingProgress.NotReady,
                TimeStamp = DateTime.UtcNow
            });

            var request = handler.Requests.Single();
            Assert.Equal("https://platform.example.com/li/1/scores?x=y", request.RequestUri.AbsoluteUri);
            Assert.Equal(Constants.MediaTypes.Score, request.Content.Headers.ContentType.MediaType);
            var body = handler.Bodies.Single();
            Assert.DoesNotContain("scoreGiven", body);
            Assert.DoesNotContain("null", body);
        }

        [Fact]
        public async Task ThrowTheProblemDetails_OfAFailedCall()
        {
            var handler = new StubHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent("{\"title\":\"Line item not found\",\"detail\":\"No line item 7.\",\"status\":404}",
                    Encoding.UTF8, "application/problem+json")
            });

            var ex = await Assert.ThrowsAsync<LtiServiceException>(() =>
                new HttpClient(handler).GetLineItemAsync("t", "https://platform.example.com/li/7"));

            Assert.Equal(HttpStatusCode.NotFound, ex.StatusCode);
            Assert.Equal("Line item not found", ex.Title);
            Assert.Equal("No line item 7.", ex.Detail);
            Assert.Contains("404", ex.Message);
            Assert.Contains("No line item 7.", ex.Message);
        }

        [Fact]
        public async Task ReportTheChallenge_WhenAFailedCallHasNoBody()
        {
            var handler = new StubHandler(_ =>
            {
                var response = new HttpResponseMessage(HttpStatusCode.Forbidden);
                response.Headers.WwwAuthenticate.ParseAdd("Bearer error=\"insufficient_scope\"");
                return response;
            });

            var ex = await Assert.ThrowsAsync<LtiServiceException>(() =>
                new HttpClient(handler).GetLineItemsAsync("t", LineItems));

            Assert.Contains("insufficient_scope", ex.Message);
            Assert.Null(ex.Title);
        }

        [Theory]
        [InlineData("<https://a.example/2>; rel=\"next\"", "https://a.example/2")]
        [InlineData("<https://a.example/1>; rel=\"prev\", <https://a.example/3>; rel=\"last next\"", "https://a.example/3")]
        [InlineData("<https://a.example/2>; title=\"x\"; REL=next", "https://a.example/2")]
        [InlineData("<https://a.example/2>; rel=\"prev\"", null)]
        public void FindTheNextLink(string header, string expected)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://a.example/1")
            };
            response.Headers.TryAddWithoutValidation("Link", header);

            Assert.Equal(expected, response.GetNextPageUrl()?.AbsoluteUri);
        }

        private static HttpResponseMessage Json(string json, string link = null)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(json)};
            if (link != null) response.Headers.TryAddWithoutValidation("Link", link);
            return response;
        }

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) => _respond = respond;

            public List<HttpRequestMessage> Requests { get; } = new();

            public List<string> Bodies { get; } = new();

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                Requests.Add(request);
                if (request.Content != null) Bodies.Add(await request.Content.ReadAsStringAsync(cancellationToken));
                var response = _respond(request);
                response.RequestMessage = request;
                return response;
            }
        }
    }
}
