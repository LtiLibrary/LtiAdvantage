using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LtiAdvantage.IdentityModel.Validation;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace LtiAdvantage.UnitTests.Validation
{
    public class LtiTokenValidatorShould
    {
        private const string Issuer = "https://platform.example.com";
        private const string ClientId = "client-1";
        private const string JwksUri = "https://platform.example.com/.well-known/jwks.json";

        [Fact]
        public async Task AcceptAWellFormedToken()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            var result = await validator.ValidateAsync(platform.CreateIdToken());

            Assert.True(result.IsValid, result.Error);
            Assert.Null(result.Error);
            Assert.Equal("LtiResourceLinkRequest", result.MessageType);
            Assert.Equal("user-1", result.Payload.Sub);
        }

        /// <summary>
        /// The payload is the handoff point to the typed message classes.
        /// </summary>
        [Fact]
        public async Task ReturnAPayloadTheMessageTypesCanConsume()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            var result = await validator.ValidateAsync(platform.CreateIdToken());

            var request = new LtiAdvantage.Lti.LtiResourceLinkRequest(result.Payload);
            Assert.Equal("dep-1", request.DeploymentId);
            Assert.Equal("user-1", request.UserId);
        }

        [Fact]
        public async Task RejectAnUnreadableToken()
        {
            var validator = CreateValidator(new FakePlatform("key-1"));

            Assert.False((await validator.ValidateAsync(null)).IsValid);
            Assert.False((await validator.ValidateAsync("")).IsValid);
            Assert.False((await validator.ValidateAsync("not-a-jwt")).IsValid);
        }

        [Fact]
        public async Task RejectAnUnexpectedIssuer()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            var result = await validator.ValidateAsync(platform.CreateIdToken(issuer: "https://evil.example.com"));

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task RejectATokenForAnotherAudience()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            var result = await validator.ValidateAsync(platform.CreateIdToken(audiences: new[] {"someone-else"}));

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task RejectAnExpiredToken()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            // Beyond the default five minutes of clock skew.
            var expired = platform.CreateIdToken(expires: DateTime.UtcNow.AddMinutes(-30));

            Assert.False((await validator.ValidateAsync(expired)).IsValid);
        }

        [Fact]
        public async Task RejectATokenSignedByAnUntrustedKey()
        {
            var platform = new FakePlatform("key-1");
            var impostor = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            // Same key id, different key: the signature must not verify.
            var result = await validator.ValidateAsync(impostor.CreateIdToken());

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task RejectATamperedToken()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            var token = platform.CreateIdToken();
            var tampered = token.Substring(0, token.LastIndexOf('.') + 1) + "AAAA";

            Assert.False((await validator.ValidateAsync(tampered)).IsValid);
        }

        [Theory]
        [InlineData("1.1.0")]
        [InlineData(null)]
        public async Task RejectAnUnsupportedLtiVersion(string version)
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            var result = await validator.ValidateAsync(platform.CreateIdToken(version: version));

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task RejectATokenWithoutAMessageTypeOrDeploymentId()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            Assert.False((await validator.ValidateAsync(platform.CreateIdToken(messageType: null))).IsValid);
            Assert.False((await validator.ValidateAsync(platform.CreateIdToken(deploymentId: null))).IsValid);
        }

        [Fact]
        public async Task RejectATokenWithoutANonce()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            var result = await validator.ValidateAsync(platform.CreateIdToken(nonce: null));

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task AcceptATokenWithoutANonce_WhenNotRequired()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options => options.RequireNonce = false);

            var result = await validator.ValidateAsync(platform.CreateIdToken(nonce: null));

            Assert.True(result.IsValid, result.Error);
        }

        /// <summary>
        /// A nonce may be used at most once, which is the whole point of the claim.
        /// </summary>
        [Fact]
        public async Task RejectAReplayedNonce()
        {
            var platform = new FakePlatform("key-1");
            var store = new InMemoryNonceStore();
            var validator = CreateValidator(platform, options => options.NonceStore = store);

            var token = platform.CreateIdToken(nonce: "nonce-once");

            Assert.True((await validator.ValidateAsync(token)).IsValid);

            var replay = await validator.ValidateAsync(token);
            Assert.False(replay.IsValid);
            Assert.Contains("nonce", replay.Error, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// With several audiences, azp decides which one the token was minted for. Accepting a
        /// token without it would let a token issued for a sibling tool through.
        /// </summary>
        [Fact]
        public async Task RejectMultipleAudiencesWithoutAValidAzp()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);
            var audiences = new[] {ClientId, "other-tool"};

            var noAzp = await validator.ValidateAsync(platform.CreateIdToken(audiences: audiences));
            Assert.False(noAzp.IsValid);

            var wrongAzp = await validator.ValidateAsync(
                platform.CreateIdToken(audiences: audiences, azp: "other-tool"));
            Assert.False(wrongAzp.IsValid);

            var rightAzp = await validator.ValidateAsync(
                platform.CreateIdToken(audiences: audiences, azp: ClientId));
            Assert.True(rightAzp.IsValid, rightAzp.Error);
        }

        /// <summary>
        /// The JWKS is fetched once and reused; refetching per launch would put the platform's
        /// key endpoint in the hot path of every login.
        /// </summary>
        [Fact]
        public async Task CacheTheJwks()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            for (var i = 0; i < 5; i++)
            {
                Assert.True((await validator.ValidateAsync(platform.CreateIdToken())).IsValid);
            }

            Assert.Equal(1, platform.FetchCount);
        }

        /// <summary>
        /// When the platform rotates its signing key, a token with the new key id must be
        /// accepted without waiting for the cache to expire.
        /// </summary>
        [Fact]
        public async Task RefetchTheJwks_WhenTheKeyIdIsUnknown()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options =>
            {
                options.JwksCacheDuration = TimeSpan.FromHours(24);
                options.MinimumRefreshInterval = TimeSpan.Zero;
            });

            Assert.True((await validator.ValidateAsync(platform.CreateIdToken())).IsValid);
            Assert.Equal(1, platform.FetchCount);

            platform.RotateKey("key-2");

            var afterRotation = await validator.ValidateAsync(platform.CreateIdToken());

            Assert.True(afterRotation.IsValid, afterRotation.Error);
            Assert.Equal(2, platform.FetchCount);
        }

        /// <summary>
        /// The early refresh is rate limited, so a stream of tokens carrying bogus key ids
        /// cannot be turned into a stream of requests against the platform.
        /// </summary>
        [Fact]
        public async Task NotRefetchTheJwksRepeatedly_ForAnUnknownKeyId()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options =>
                options.MinimumRefreshInterval = TimeSpan.FromMinutes(5));

            Assert.True((await validator.ValidateAsync(platform.CreateIdToken())).IsValid);

            var impostor = new FakePlatform("bogus-key");
            for (var i = 0; i < 10; i++)
            {
                Assert.False((await validator.ValidateAsync(impostor.CreateIdToken())).IsValid);
            }

            Assert.Equal(1, platform.FetchCount);
        }

        /// <summary>
        /// A JWKS outage must not take every launch down with it: keys fetched earlier are still
        /// served, and failed refreshes are rate limited like successful ones.
        /// </summary>
        [Fact]
        public async Task KeepServingCachedKeys_WhileTheJwksEndpointIsDown()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options =>
            {
                options.JwksCacheDuration = TimeSpan.Zero; // every call wants a refresh
                options.MinimumRefreshInterval = TimeSpan.FromMinutes(5);
            });

            Assert.True((await validator.ValidateAsync(platform.CreateIdToken())).IsValid);
            platform.JwksDown = true;

            for (var i = 0; i < 5; i++)
            {
                var result = await validator.ValidateAsync(platform.CreateIdToken());
                Assert.True(result.IsValid, result.Error);
            }

            // Only the first fetch: the cache entry is within the minimum interval.
            Assert.Equal(1, platform.FetchCount);
        }

        [Fact]
        public async Task KeepServingCachedKeys_WhenARefreshFails()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options =>
            {
                options.JwksCacheDuration = TimeSpan.Zero;
                options.MinimumRefreshInterval = TimeSpan.Zero;
            });

            Assert.True((await validator.ValidateAsync(platform.CreateIdToken())).IsValid);
            platform.JwksDown = true;

            var result = await validator.ValidateAsync(platform.CreateIdToken());

            Assert.True(result.IsValid, result.Error);
            // The expired set triggers a background refresh, which fails; the cached keys stay.
            await WaitUntil(() => platform.FetchCount == 2);
            var afterFailedRefresh = await validator.ValidateAsync(platform.CreateIdToken());
            Assert.True(afterFailedRefresh.IsValid, afterFailedRefresh.Error);
        }

        private static async Task WaitUntil(Func<bool> condition)
        {
            for (var i = 0; i < 200 && !condition(); i++) await Task.Delay(10);
            Assert.True(condition(), "condition not reached");
        }

        /// <summary>
        /// When the cache has expired but already holds the caller's key, the caller gets the
        /// cached keys at once; the refresh runs in the background, so a hanging JWKS endpoint
        /// does not stall every launch.
        /// </summary>
        [Fact]
        public async Task NotWaitForAnExpiryRefresh_WhenTheKeyIsCached()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options =>
            {
                options.JwksCacheDuration = TimeSpan.Zero;
                options.MinimumRefreshInterval = TimeSpan.Zero;
            });
            Assert.True((await validator.ValidateAsync(platform.CreateIdToken())).IsValid);

            platform.HoldJwks = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var launch = validator.ValidateAsync(platform.CreateIdToken());
            var finished = await Task.WhenAny(launch, Task.Delay(TimeSpan.FromSeconds(5)));

            Assert.Same(launch, finished);
            Assert.True(launch.Result.IsValid, launch.Result.Error);
            platform.HoldJwks.SetResult(true);
        }

        /// <summary>
        /// A maximum staleness below the cache duration is raised to it, so keys still within
        /// their cache lifetime are never thrown away.
        /// </summary>
        [Fact]
        public async Task NotDropFreshKeys_WhenMaximumStalenessIsBelowTheCacheDuration()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options =>
            {
                options.JwksCacheDuration = TimeSpan.FromHours(1);
                options.MaximumKeyStaleness = TimeSpan.Zero;
            });

            Assert.True((await validator.ValidateAsync(platform.CreateIdToken())).IsValid);
            await Task.Delay(20);
            platform.JwksDown = true;
            var result = await validator.ValidateAsync(platform.CreateIdToken());

            Assert.True(result.IsValid, result.Error);
            Assert.Equal(1, platform.FetchCount);
        }

        /// <summary>
        /// The key id comes from whoever sent the token, so a JWKS fetch failure it triggers must
        /// come back as a failed validation, not an exception (which would be a 500 at the launch
        /// endpoint), and must not be retried on every token.
        /// </summary>
        [Fact]
        public async Task ReportAFailure_NotAnException_WhenTheJwksCannotBeFetched()
        {
            var platform = new FakePlatform("key-1") { JwksDown = true };
            var validator = CreateValidator(platform, options =>
                options.MinimumRefreshInterval = TimeSpan.FromMinutes(5));

            for (var i = 0; i < 5; i++)
            {
                var result = await validator.ValidateAsync(platform.CreateIdToken());
                Assert.False(result.IsValid);
                Assert.Contains("signing keys could not be fetched", result.Error);
                Assert.DoesNotContain(JwksUri, result.Error);
                Assert.NotNull(result.Exception);
            }

            Assert.Equal(1, platform.FetchCount);
        }

        /// <summary>
        /// OpenID Connect Core 3.1.3.7: whenever azp is present it must be this client, even with
        /// a single audience.
        /// </summary>
        [Fact]
        public async Task RejectAWrongAzp_EvenWithASingleAudience()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            var result = await validator.ValidateAsync(platform.CreateIdToken(azp: "other-tool"));

            Assert.False(result.IsValid);
            Assert.Contains("azp", result.Error);
        }

        [Fact]
        public async Task RejectATokenWithoutIat()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            var result = await validator.ValidateAsync(platform.CreateIdToken(includeIssuedAt: false));

            Assert.False(result.IsValid);
            Assert.Contains("iat", result.Error);
        }

        [Fact]
        public async Task RejectAnIatInTheFuture_BeyondTheClockSkew()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options => options.ClockSkew = TimeSpan.FromMinutes(5));

            var withinSkew = await validator.ValidateAsync(
                platform.CreateIdToken(issuedAt: DateTime.UtcNow.AddMinutes(2)));
            var beyondSkew = await validator.ValidateAsync(
                platform.CreateIdToken(issuedAt: DateTime.UtcNow.AddMinutes(30), expires: DateTime.UtcNow.AddMinutes(40)));

            Assert.True(withinSkew.IsValid, withinSkew.Error);
            Assert.False(beyondSkew.IsValid);
            Assert.Contains("iat", beyondSkew.Error);
        }

        /// <summary>
        /// A forged token with an unknown key id triggers a refetch. If the platform is slow to
        /// answer, launches whose key is already cached must not wait behind it.
        /// </summary>
        [Fact]
        public async Task NotBlockCachedKeys_WhileARefreshIsInFlight()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options => options.MinimumRefreshInterval = TimeSpan.Zero);
            Assert.True((await validator.ValidateAsync(platform.CreateIdToken())).IsValid);

            platform.HoldJwks = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var impostor = new FakePlatform("bogus-key");
            var stuck = validator.ValidateAsync(impostor.CreateIdToken());

            var legit = validator.ValidateAsync(platform.CreateIdToken());
            var finished = await Task.WhenAny(legit, Task.Delay(TimeSpan.FromSeconds(5)));

            Assert.Same(legit, finished);
            Assert.True(legit.Result.IsValid, legit.Result.Error);
            Assert.False(stuck.IsCompleted);

            platform.HoldJwks.SetResult(true);
            Assert.False((await stuck).IsValid);
        }

        [Fact]
        public async Task ReportAFailure_ForANonNumericIat()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform);

            var result = await validator.ValidateAsync(platform.CreateIdToken(rawIssuedAt: "not-a-number"));

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ReportAFailure_WhenTheNonceStoreThrows()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options => options.NonceStore = new ThrowingNonceStore());

            var result = await validator.ValidateAsync(platform.CreateIdToken());

            Assert.False(result.IsValid);
            Assert.Equal("nonce store unavailable", result.Exception?.Message);
            Assert.DoesNotContain("nonce store unavailable", result.Error);
        }

        private sealed class ThrowingNonceStore : INonceStore
        {
            public Task<bool> TryConsumeAsync(string nonce, DateTime expiresAt, CancellationToken cancellationToken = default) =>
                throw new InvalidOperationException("nonce store unavailable");
        }

        /// <summary>
        /// One client id can have several deployments; when the tool lists the ones it registered,
        /// any other deployment id is rejected.
        /// </summary>
        [Fact]
        public async Task RejectAnUnregisteredDeploymentId_WhenDeploymentsAreConfigured()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options => options.DeploymentIds = new[] {"dep-1"});

            var registered = await validator.ValidateAsync(platform.CreateIdToken(deploymentId: "dep-1"));
            var unregistered = await validator.ValidateAsync(platform.CreateIdToken(deploymentId: "dep-2"));

            Assert.True(registered.IsValid, registered.Error);
            Assert.False(unregistered.IsValid);
            Assert.Contains("deployment_id", unregistered.Error);
        }

        /// <summary>
        /// Without a nonce store replays cannot be detected, so that must be a deliberate choice,
        /// not a silent default.
        /// </summary>
        [Fact]
        public void RequireANonceStore_UnlessReuseIsExplicitlyAllowed()
        {
            var platform = new FakePlatform("key-1");
            LtiTokenValidationOptions Options() => new() {Issuer = Issuer, ClientId = ClientId, JwksUri = JwksUri};

            var ex = Assert.Throws<ArgumentException>(() => new LtiTokenValidator(Options(), platform.CreateClient()));
            Assert.Contains("NonceStore", ex.Message);

            var withStore = Options();
            withStore.NonceStore = new InMemoryNonceStore();
            _ = new LtiTokenValidator(withStore, platform.CreateClient());

            var optedOut = Options();
            optedOut.AllowNonceReuse = true;
            _ = new LtiTokenValidator(optedOut, platform.CreateClient());

            var nonceNotRequired = Options();
            nonceNotRequired.RequireNonce = false;
            _ = new LtiTokenValidator(nonceNotRequired, platform.CreateClient());
        }

        /// <summary>
        /// A platform that publishes keys without a kid must not make every token look like a key
        /// rotation, which would turn the cache into a poll every MinimumRefreshInterval.
        /// </summary>
        [Fact]
        public async Task NotRefetch_WhenThePlatformPublishesNoKeyIds()
        {
            var platform = new FakePlatform("key-1") {PublishKeyIds = false};
            var validator = CreateValidator(platform, options => options.MinimumRefreshInterval = TimeSpan.Zero);

            for (var i = 0; i < 3; i++)
            {
                await validator.ValidateAsync(platform.CreateIdToken());
            }

            Assert.Equal(1, platform.FetchCount);
        }

        /// <summary>
        /// Stale keys are only served up to a maximum age: past it, a revoked key must stop being
        /// trusted even if the JWKS still cannot be fetched.
        /// </summary>
        [Fact]
        public async Task StopTrustingStaleKeys_AfterTheMaximumStaleness()
        {
            var platform = new FakePlatform("key-1");
            var validator = CreateValidator(platform, options =>
            {
                options.JwksCacheDuration = TimeSpan.Zero;
                options.MinimumRefreshInterval = TimeSpan.Zero;
                options.MaximumKeyStaleness = TimeSpan.Zero;
            });

            Assert.True((await validator.ValidateAsync(platform.CreateIdToken())).IsValid);
            platform.JwksDown = true;
            await Task.Delay(20);

            var result = await validator.ValidateAsync(platform.CreateIdToken());

            Assert.False(result.IsValid);
        }

        /// <summary>Validators for several client ids on one platform can share one key cache.</summary>
        [Fact]
        public async Task ShareAJwksCache_BetweenValidators()
        {
            var platform = new FakePlatform("key-1");
            var cache = new JwksCache(platform.CreateClient(), TimeSpan.FromHours(1), TimeSpan.FromMinutes(5));
            LtiTokenValidator ValidatorFor(string clientId) => new(new LtiTokenValidationOptions
            {
                Issuer = Issuer, ClientId = clientId, JwksUri = JwksUri, AllowNonceReuse = true
            }, cache);

            var first = await ValidatorFor(ClientId).ValidateAsync(platform.CreateIdToken());
            var second = await ValidatorFor("client-2").ValidateAsync(platform.CreateIdToken(audiences: new[] {"client-2"}));

            Assert.True(first.IsValid, first.Error);
            Assert.True(second.IsValid, second.Error);
            Assert.Equal(1, platform.FetchCount);
        }

        /// <summary>The nonce store is told when the token expires, so it knows how long to keep the nonce.</summary>
        [Fact]
        public async Task PassTheTokenExpiry_ToTheNonceStore()
        {
            var platform = new FakePlatform("key-1");
            var store = new InMemoryNonceStore();
            var validator = CreateValidator(platform, options => options.NonceStore = store);
            var expires = DateTime.UtcNow.AddMinutes(7);

            Assert.True((await validator.ValidateAsync(platform.CreateIdToken(expires: expires))).IsValid);

            Assert.NotNull(store.LastExpiresAt);
            Assert.True(Math.Abs((store.LastExpiresAt.Value - expires).TotalSeconds) < 2);
        }

        [Fact]
        public void RequireTheRegistrationDetails()
        {
            var platform = new FakePlatform("key-1");

            Assert.Throws<ArgumentNullException>(() => new LtiTokenValidator(null, platform.CreateClient()));
            Assert.Throws<ArgumentNullException>(() => new LtiTokenValidator(new LtiTokenValidationOptions
            {
                Issuer = Issuer, ClientId = ClientId, JwksUri = JwksUri
            }, (HttpClient) null));
            Assert.Throws<ArgumentNullException>(() => new LtiTokenValidator(new LtiTokenValidationOptions
            {
                Issuer = Issuer, ClientId = ClientId, JwksUri = JwksUri
            }, (JwksCache) null));

            Assert.Throws<ArgumentException>(() => new LtiTokenValidator(new LtiTokenValidationOptions
            {
                ClientId = ClientId, JwksUri = JwksUri
            }, platform.CreateClient()));
            Assert.Throws<ArgumentException>(() => new LtiTokenValidator(new LtiTokenValidationOptions
            {
                Issuer = Issuer, JwksUri = JwksUri
            }, platform.CreateClient()));
            Assert.Throws<ArgumentException>(() => new LtiTokenValidator(new LtiTokenValidationOptions
            {
                Issuer = Issuer, ClientId = ClientId
            }, platform.CreateClient()));
        }

        private static LtiTokenValidator CreateValidator(FakePlatform platform,
            Action<LtiTokenValidationOptions> configure = null)
        {
            var options = new LtiTokenValidationOptions
            {
                Issuer = Issuer,
                ClientId = ClientId,
                JwksUri = JwksUri,
                // Most tests reuse one nonce; the replay tests configure a real store.
                AllowNonceReuse = true
            };

            configure?.Invoke(options);

            return new LtiTokenValidator(options, platform.CreateClient());
        }

        /// <summary>
        /// A platform that signs ID tokens and serves the matching JWKS over a stubbed transport.
        /// </summary>
        private sealed class FakePlatform
        {
            private readonly RSA _rsa = RSA.Create(2048);
            private string _keyId;

            public FakePlatform(string keyId) => _keyId = keyId;

            public int FetchCount { get; private set; }

            /// <summary>While true, the JWKS endpoint answers 503.</summary>
            public bool JwksDown { get; set; }

            /// <summary>When false, the JWKS lists the key without a kid (tokens still carry one).</summary>
            public bool PublishKeyIds { get; set; } = true;

            /// <summary>When set, the JWKS endpoint waits for this before answering.</summary>
            public TaskCompletionSource<bool> HoldJwks { get; set; }

            public void RotateKey(string keyId) => _keyId = keyId;

            public HttpClient CreateClient() => new(new StubHandler(this));

            public string CreateIdToken(
                string issuer = Issuer,
                string[] audiences = null,
                string azp = null,
                string nonce = "nonce-1",
                string version = "1.3.0",
                string messageType = "LtiResourceLinkRequest",
                string deploymentId = "dep-1",
                DateTime? expires = null,
                DateTime? issuedAt = null,
                bool includeIssuedAt = true,
                object rawIssuedAt = null)
            {
                var payload = new JwtPayload
                {
                    ["iss"] = issuer,
                    ["sub"] = "user-1",
                    ["exp"] = EpochTime.GetIntDate(expires ?? DateTime.UtcNow.AddMinutes(5))
                };
                if (rawIssuedAt != null) payload["iat"] = rawIssuedAt;
                else if (includeIssuedAt) payload["iat"] = EpochTime.GetIntDate(issuedAt ?? DateTime.UtcNow);

                payload["aud"] = audiences == null ? new[] {ClientId} : audiences;
                if (azp != null) payload["azp"] = azp;
                if (nonce != null) payload["nonce"] = nonce;
                if (version != null) payload["https://purl.imsglobal.org/spec/lti/claim/version"] = version;
                if (messageType != null)
                    payload["https://purl.imsglobal.org/spec/lti/claim/message_type"] = messageType;
                if (deploymentId != null)
                    payload["https://purl.imsglobal.org/spec/lti/claim/deployment_id"] = deploymentId;

                var credentials = new SigningCredentials(
                    new RsaSecurityKey(_rsa) {KeyId = _keyId}, SecurityAlgorithms.RsaSha256);

                return new JwtSecurityTokenHandler().WriteToken(
                    new JwtSecurityToken(new JwtHeader(credentials), payload));
            }

            private string CreateJwks()
            {
                // Publish public components only, as a real platform must.
                var publicKey = new RsaSecurityKey(_rsa.ExportParameters(false)) {KeyId = PublishKeyIds ? _keyId : null};
                var jwk = JsonWebKeyConverter.ConvertFromRSASecurityKey(publicKey);
                jwk.Use = "sig";
                jwk.Alg = SecurityAlgorithms.RsaSha256;

                return JsonSerializer.Serialize(new {keys = new[] {jwk}});
            }

            private sealed class StubHandler : HttpMessageHandler
            {
                private readonly FakePlatform _platform;

                public StubHandler(FakePlatform platform) => _platform = platform;

                protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                    CancellationToken cancellationToken)
                {
                    Assert.Equal(JwksUri, request.RequestUri.ToString());
                    _platform.FetchCount++;

                    if (_platform.HoldJwks != null) await _platform.HoldJwks.Task;

                    if (_platform.JwksDown)
                        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(_platform.CreateJwks())
                    };
                }
            }
        }

        private sealed class InMemoryNonceStore : INonceStore
        {
            private readonly HashSet<string> _seen = new(StringComparer.Ordinal);

            public DateTime? LastExpiresAt { get; private set; }

            public Task<bool> TryConsumeAsync(string nonce, DateTime expiresAt, CancellationToken cancellationToken = default)
            {
                LastExpiresAt = expiresAt;
                lock (_seen)
                {
                    return Task.FromResult(_seen.Add(nonce));
                }
            }
        }
    }
}
