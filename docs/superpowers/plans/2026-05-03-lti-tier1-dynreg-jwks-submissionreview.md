# LTI Advantage Tier 1 Implementation Plan — Submission Review, JWKS Publishing, Dynamic Registration

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add three independently-shippable LTI 1.3 / Advantage features that the library currently lacks and that are commonly requested by integrators: (A) Submission Review 1.0 message type, (B) JWKS publishing endpoint, (C) Dynamic Registration 1.0 (both tool side and platform side).

**Architecture:** Each feature follows the existing repo conventions:
- Domain models live under `src/LtiAdvantage/<FeatureArea>/` (netstandard2.0, `JsonPropertyName` attributes from `System.Text.Json.Serialization`).
- ASP.NET Core controller bases live under `src/LtiAdvantage.AspNetCore/<FeatureArea>/` with `[ApiController]` + `Authorize(Policy = …)` + abstract `On…Async` extension points.
- Tool-side helpers (HttpClient extensions) live under `src/LtiAdvantage.IdentityModel/Client/`.
- Constants are added to `src/LtiAdvantage/Constants.cs`.
- xUnit unit tests under `test/LtiAdvantage.UnitTests/`, integration tests under `test/LtiAdvantage.IntegrationTests/` using `TestServer` and the existing `Startup.cs` / `TestAuthHandler`.

**Phases:**
- **Phase A — Submission Review 1.0** (smallest; mostly models + claims). Spec: https://www.imsglobal.org/spec/lti-sr/v1p0
- **Phase B — JWKS publishing endpoint** (small; controller base + key store interface). Spec: https://www.imsglobal.org/spec/security/v1p0/#authentication-response-validation
- **Phase C — LTI Dynamic Registration 1.0** (largest; both tool registration client and platform registration controller). Spec: https://www.imsglobal.org/spec/lti-dr/v1p0

Each phase ends with a release-able state. A reasonable shipping plan is to release each phase as its own minor version bump.

**Tech Stack:** .NET (netstandard2.0 core, net8.0/net10.0 ASP.NET Core), xUnit, System.Text.Json, Microsoft.IdentityModel.Tokens, BouncyCastle.NetCore (already a dep).

---

## File Structure

### Phase A — Submission Review

```
src/LtiAdvantage/
├── Constants.cs                                      (modify: add SubmissionReview consts)
├── SubmissionReview/                                  (new dir)
│   ├── ForUserClaimValueType.cs                       (new)
│   └── SubmissionReviewClaimValueType.cs              (new — sr endpoint claim)
└── Lti/
    └── LtiSubmissionReviewRequest.cs                  (new)

test/LtiAdvantage.UnitTests/
├── ReferenceJson/
│   └── LtiSubmissionReviewRequest.json                (new — copy from spec example)
├── LtiAdvantage.UnitTests.csproj                     (modify: register reference json)
└── SubmissionReview/                                  (new dir)
    └── LtiSubmissionReviewRequestShould.cs            (new)
```

### Phase B — JWKS Publishing

```
src/LtiAdvantage/
├── Constants.cs                                      (modify: add Jwks endpoint const)
└── Jwks/                                              (new dir)
    └── IJwksKeyStore.cs                               (new — abstraction over key material)

src/LtiAdvantage.AspNetCore/
└── Jwks/                                              (new dir)
    ├── IJwksController.cs                             (new)
    └── JwksControllerBase.cs                          (new)

test/LtiAdvantage.IntegrationTests/
├── Controllers/
│   └── JwksController.cs                              (new — stub for tests)
└── Jwks/                                              (new dir)
    └── JwksControllerShould.cs                        (new)
```

### Phase C — Dynamic Registration

```
src/LtiAdvantage/
├── Constants.cs                                      (modify: add DynReg claims, scopes, message types)
└── DynamicRegistration/                               (new dir)
    ├── PlatformOpenIdConfiguration.cs                 (new — RFC8414 + LTI extensions)
    ├── LtiPlatformConfiguration.cs                    (new — LTI-specific claim of openid config)
    ├── ToolConfiguration.cs                           (new — registration request body)
    ├── LtiToolConfiguration.cs                        (new — LTI claim of registration body)
    ├── MessageDescriptor.cs                           (new — supported messages incl. types/placements)
    └── RegistrationResponse.cs                        (new)

src/LtiAdvantage.IdentityModel/
└── Client/
    └── HttpClientDynamicRegistrationExtensions.cs     (new — tool-side HttpClient extension)

src/LtiAdvantage.AspNetCore/
└── DynamicRegistration/                               (new dir)
    ├── IDynamicRegistrationController.cs              (new)
    ├── DynamicRegistrationControllerBase.cs           (new — platform-side endpoint)
    └── RegisterToolRequest.cs                         (new)

test/LtiAdvantage.UnitTests/
├── ReferenceJson/
│   ├── PlatformOpenIdConfiguration.json               (new)
│   └── ToolConfiguration.json                         (new)
└── DynamicRegistration/                               (new dir)
    ├── PlatformOpenIdConfigurationShould.cs           (new)
    └── ToolConfigurationShould.cs                     (new)

test/LtiAdvantage.IntegrationTests/
├── Controllers/
│   └── DynamicRegistrationController.cs               (new — stub for tests)
└── DynamicRegistration/                               (new dir)
    └── DynamicRegistrationControllerShould.cs         (new)
```

---

## Spec Coverage Map (verify each item maps to a task before declaring complete)

| Spec section | Plan task |
|---|---|
| LTI-SR §3.1 message type & version claim | A2 |
| LTI-SR §3.2 `for_user` claim with `user_id`, `name`, `email`, `roles` | A1, A3 |
| LTI-SR §3.3 SR launch URL exposed via AGS lineitem `submissionReview` | A4 |
| Security §5 platform JWKS publication | B2 |
| Security §5.2.1 `kid`, `alg`, `use=sig` on each key | B1, B2 |
| LTI-DR §3 OIDC discovery `openid_configuration` from platform | C1, C2 |
| LTI-DR §3.5 `https://purl.imsglobal.org/spec/lti-platform-configuration` claim on discovery | C2 |
| LTI-DR §3.6 tool `https://purl.imsglobal.org/spec/lti-tool-configuration` claim on registration request | C3 |
| LTI-DR §4 Bearer-token-protected registration endpoint | C5, C6 |
| LTI-DR §4.5 platform returns `client_id` plus echoed config | C5 |

---

# Phase A — Submission Review 1.0

### Task A1: Add Submission Review constants

**Files:**
- Modify: `src/LtiAdvantage/Constants.cs`

- [ ] **Step 1: Add the new constants**

In `Constants.cs`, inside `class Lti`, add after `LtiResourceLinkRequestMessageType`:

```csharp
/// <summary>
/// The message type of an LtiSubmissionReviewRequest.
/// </summary>
public const string LtiSubmissionReviewRequestMessageType = "LtiSubmissionReviewRequest";
```

Inside `class LtiClaims`, add (alphabetical order, after `ErrorMessage`):

```csharp
/// <summary>
/// The user whose submission is being reviewed (Submission Review 1.0 §3.2).
/// </summary>
public const string ForUser = "https://purl.imsglobal.org/spec/lti-sr/claim/for_user";
```

- [ ] **Step 2: Build**

Run: `dotnet build src/LtiAdvantage/LtiAdvantage.csproj`
Expected: Build succeeded.

- [ ] **Step 3: Commit**

```bash
git add src/LtiAdvantage/Constants.cs
git commit -m "feat(sr): add Submission Review message type and for_user claim constants"
```

### Task A2: Create `LtiSubmissionReviewRequest` with failing test

**Files:**
- Create: `src/LtiAdvantage/Lti/LtiSubmissionReviewRequest.cs`
- Create: `test/LtiAdvantage.UnitTests/SubmissionReview/LtiSubmissionReviewRequestShould.cs`
- Create: `src/LtiAdvantage/SubmissionReview/ForUserClaimValueType.cs`

- [ ] **Step 1: Write the failing test**

Create `test/LtiAdvantage.UnitTests/SubmissionReview/LtiSubmissionReviewRequestShould.cs`:

```csharp
using LtiAdvantage.Lti;
using LtiAdvantage.SubmissionReview;
using Xunit;

namespace LtiAdvantage.UnitTests.SubmissionReview
{
    public class LtiSubmissionReviewRequestShould
    {
        [Fact]
        public void HaveCorrectMessageTypeAndVersion()
        {
            var request = new LtiSubmissionReviewRequest();

            Assert.True(request.TryGetValue(
                "https://purl.imsglobal.org/spec/lti/claim/message_type", out var messageType));
            Assert.Equal("LtiSubmissionReviewRequest", messageType);

            Assert.True(request.TryGetValue(
                "https://purl.imsglobal.org/spec/lti/claim/version", out var version));
            Assert.Equal("1.3.0", version);
        }

        [Fact]
        public void RoundTripForUserClaim()
        {
            var request = new LtiSubmissionReviewRequest
            {
                ForUser = new ForUserClaimValueType
                {
                    UserId = "abc-123",
                    Name = "Jane Doe",
                    Email = "jane@example.edu"
                }
            };

            Assert.Equal("abc-123", request.ForUser.UserId);
            Assert.Equal("Jane Doe", request.ForUser.Name);
        }
    }
}
```

- [ ] **Step 2: Run the tests; verify they fail to compile**

Run: `dotnet test test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj --filter FullyQualifiedName~LtiSubmissionReviewRequestShould`
Expected: FAIL (compilation error — `LtiSubmissionReviewRequest` and `ForUserClaimValueType` not found).

- [ ] **Step 3: Implement `ForUserClaimValueType`**

Create `src/LtiAdvantage/SubmissionReview/ForUserClaimValueType.cs`:

```csharp
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.SubmissionReview
{
    /// <summary>
    /// The user whose submission is being reviewed.
    /// See https://www.imsglobal.org/spec/lti-sr/v1p0/#for-user-claim
    /// </summary>
    public class ForUserClaimValueType
    {
        /// <summary>
        /// The user_id of the reviewed user.
        /// </summary>
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// The reviewed user's full name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The reviewed user's given (first) name.
        /// </summary>
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        /// <summary>
        /// The reviewed user's family (last) name.
        /// </summary>
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        /// <summary>
        /// The reviewed user's email.
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// The roles of the reviewed user in the context.
        /// </summary>
        [JsonPropertyName("roles")]
        public IList<string> Roles { get; set; }

        /// <summary>
        /// The person sourcedId of the reviewed user.
        /// </summary>
        [JsonPropertyName("person_sourcedid")]
        public string PersonSourcedId { get; set; }
    }
}
```

- [ ] **Step 4: Implement `LtiSubmissionReviewRequest`**

Create `src/LtiAdvantage/Lti/LtiSubmissionReviewRequest.cs`:

```csharp
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.NamesRoleProvisioningService;
using LtiAdvantage.SubmissionReview;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    /// <inheritdoc />
    /// <summary>
    /// LTI Submission Review request (https://www.imsglobal.org/spec/lti-sr/v1p0).
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class LtiSubmissionReviewRequest : LtiRequest
    {
        public LtiSubmissionReviewRequest()
        {
            MessageType = Constants.Lti.LtiSubmissionReviewRequestMessageType;
            Version = Constants.Lti.Version;
        }

        public LtiSubmissionReviewRequest(IEnumerable<Claim> claims) : base(claims) { }

        public LtiSubmissionReviewRequest(JwtPayload payload) : base(payload.Claims) { }

        /// <summary>The Assignment and Grade Services claim (the lineitem under review).</summary>
        public AssignmentGradeServicesClaimValueType AssignmentGradeServices
        {
            get => this.GetClaimValue<AssignmentGradeServicesClaimValueType>(Constants.LtiClaims.AssignmentGradeServices);
            set => this.SetClaimValue(Constants.LtiClaims.AssignmentGradeServices, value);
        }

        /// <summary>The Names and Roles Provisioning Service claim.</summary>
        public NamesRoleServiceClaimValueType NamesRoleService
        {
            get => this.GetClaimValue<NamesRoleServiceClaimValueType>(Constants.LtiClaims.NamesRoleService);
            set => this.SetClaimValue(Constants.LtiClaims.NamesRoleService, value);
        }

        /// <summary>The resource_link claim (same shape as in LtiResourceLinkRequest).</summary>
        public ResourceLinkClaimValueType ResourceLink
        {
            get => this.GetClaimValue<ResourceLinkClaimValueType>(Constants.LtiClaims.ResourceLink);
            set => this.SetClaimValue(Constants.LtiClaims.ResourceLink, value);
        }

        /// <summary>The for_user claim (the user whose submission is being reviewed).</summary>
        public ForUserClaimValueType ForUser
        {
            get => this.GetClaimValue<ForUserClaimValueType>(Constants.LtiClaims.ForUser);
            set => this.SetClaimValue(Constants.LtiClaims.ForUser, value);
        }
    }
}
```

- [ ] **Step 5: Run tests; verify they pass**

Run: `dotnet test test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj --filter FullyQualifiedName~LtiSubmissionReviewRequestShould`
Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/LtiAdvantage/SubmissionReview src/LtiAdvantage/Lti/LtiSubmissionReviewRequest.cs test/LtiAdvantage.UnitTests/SubmissionReview
git commit -m "feat(sr): add LtiSubmissionReviewRequest and ForUserClaimValueType"
```

### Task A3: Reference-JSON round-trip test

**Files:**
- Create: `test/LtiAdvantage.UnitTests/ReferenceJson/LtiSubmissionReviewRequest.json`
- Modify: `test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj`
- Modify: `test/LtiAdvantage.UnitTests/SubmissionReview/LtiSubmissionReviewRequestShould.cs`

- [ ] **Step 1: Write the reference JSON**

Create `test/LtiAdvantage.UnitTests/ReferenceJson/LtiSubmissionReviewRequest.json` (based on https://www.imsglobal.org/spec/lti-sr/v1p0/#example-message):

```json
{
  "iss": "https://platform.example.com",
  "aud": ["abcd-1234"],
  "sub": "instructor-9",
  "exp": 1933333333,
  "iat": 1933332433,
  "nonce": "nonce-1",
  "https://purl.imsglobal.org/spec/lti/claim/message_type": "LtiSubmissionReviewRequest",
  "https://purl.imsglobal.org/spec/lti/claim/version": "1.3.0",
  "https://purl.imsglobal.org/spec/lti/claim/deployment_id": "dep-1",
  "https://purl.imsglobal.org/spec/lti/claim/target_link_uri": "https://tool.example.com/review",
  "https://purl.imsglobal.org/spec/lti/claim/resource_link": { "id": "rl-1" },
  "https://purl.imsglobal.org/spec/lti/claim/roles": [
    "http://purl.imsglobal.org/vocab/lis/v2/membership#Instructor"
  ],
  "https://purl.imsglobal.org/spec/lti-sr/claim/for_user": {
    "user_id": "student-7",
    "name": "Jane Doe",
    "given_name": "Jane",
    "family_name": "Doe",
    "email": "jane@example.edu",
    "roles": ["http://purl.imsglobal.org/vocab/lis/v2/membership#Learner"]
  }
}
```

- [ ] **Step 2: Register the JSON file in the csproj**

In `test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj`, add to the `<None Remove>` group and the `<Content Include>` group (mirroring the existing entries — see how `LtiResourceLinkRequest.json` is registered):

```xml
<None Remove="ReferenceJson\LtiSubmissionReviewRequest.json" />
```

```xml
<Content Include="ReferenceJson\LtiSubmissionReviewRequest.json">
  <CopyToOutputDirectory>Always</CopyToOutputDirectory>
</Content>
```

- [ ] **Step 3: Add the round-trip test**

Append to `LtiSubmissionReviewRequestShould.cs`:

```csharp
[Fact]
public void ParseValidLtiSubmissionReviewRequest()
{
    var referenceJson = TestUtils.LoadReferenceJsonFile("LtiSubmissionReviewRequest");
    var request = System.Text.Json.JsonSerializer.Deserialize<LtiSubmissionReviewRequest>(referenceJson);
    var requestJson = System.Text.Json.JsonSerializer.Serialize(request);
    JsonAssert.Equal(referenceJson, requestJson);
}
```

Add `using LtiAdvantage.UnitTests;` at the top so `TestUtils` and `JsonAssert` resolve.

- [ ] **Step 4: Run the test; verify pass**

Run: `dotnet test test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj --filter FullyQualifiedName~LtiSubmissionReviewRequestShould`
Expected: PASS (3 tests).

If the round-trip fails because LtiRequest serializes extra null/empty fields, follow the existing `LtiResourceLinkRequestShould` pattern — `JsonAssert.Equal` already tolerates ordering differences; if it fails on missing nullable fields, populate them in the reference JSON to match.

- [ ] **Step 5: Commit**

```bash
git add test/LtiAdvantage.UnitTests/ReferenceJson/LtiSubmissionReviewRequest.json test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj test/LtiAdvantage.UnitTests/SubmissionReview/LtiSubmissionReviewRequestShould.cs
git commit -m "test(sr): add reference-JSON round-trip for LtiSubmissionReviewRequest"
```

### Task A4: Wire `submissionReview` into AGS `LineItem`

The Submission Review service exposes the SR launch URL via an extension on the AGS line item: `LineItem.submissionReview = { url, custom }`. Add this so existing AGS users can publish SR-capable line items.

**Files:**
- Modify: `src/LtiAdvantage/AssignmentGradeServices/LineItem.cs`
- Create: `src/LtiAdvantage/SubmissionReview/SubmissionReviewProperty.cs`
- Modify (or create): `test/LtiAdvantage.UnitTests/AssignmentGradeServices/LineItemShould.cs`

- [ ] **Step 1: Write a failing test**

In `test/LtiAdvantage.UnitTests/AssignmentGradeServices/LineItemShould.cs` (create file if absent):

```csharp
using System.Text.Json;
using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.SubmissionReview;
using Xunit;

namespace LtiAdvantage.UnitTests.AssignmentGradeServices
{
    public class LineItemShould
    {
        [Fact]
        public void RoundTripSubmissionReviewProperty()
        {
            var lineItem = new LineItem
            {
                Id = "https://platform.example.com/lineitems/1",
                Label = "Essay",
                ScoreMaximum = 10,
                SubmissionReviewExtension = new SubmissionReviewProperty
                {
                    Url = "https://tool.example.com/review",
                    Custom = new System.Collections.Generic.Dictionary<string, string> { ["a"] = "1" }
                }
            };

            var json = JsonSerializer.Serialize(lineItem);
            var roundTripped = JsonSerializer.Deserialize<LineItem>(json);

            Assert.NotNull(roundTripped.SubmissionReviewExtension);
            Assert.Equal("https://tool.example.com/review", roundTripped.SubmissionReviewExtension.Url);
            Assert.Equal("1", roundTripped.SubmissionReviewExtension.Custom["a"]);
        }
    }
}
```

- [ ] **Step 2: Run; verify fails to compile**

Run: `dotnet test test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj --filter FullyQualifiedName~LineItemShould`
Expected: FAIL (`SubmissionReview` does not exist on `LineItem`).

- [ ] **Step 3: Implement `SubmissionReviewProperty`**

Create `src/LtiAdvantage/SubmissionReview/SubmissionReviewProperty.cs`:

```csharp
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.SubmissionReview
{
    /// <summary>
    /// Submission Review extension on an AGS LineItem.
    /// See https://www.imsglobal.org/spec/lti-sr/v1p0/#submissionreview-extension
    /// </summary>
    public class SubmissionReviewProperty
    {
        /// <summary>
        /// The launch URL the platform should use when launching back into
        /// the tool to review a submission for this line item.
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// Optional custom parameters to include with each Submission Review launch.
        /// </summary>
        [JsonPropertyName("custom")]
        public IDictionary<string, string> Custom { get; set; }
    }
}
```

- [ ] **Step 4: Add the property to `LineItem`**

In `src/LtiAdvantage/AssignmentGradeServices/LineItem.cs`:

(a) add `using LtiAdvantage.SubmissionReview;` to the existing usings (alongside `using System;` and `using System.Text.Json.Serialization;`).

(b) after the `Tag` property, add:

```csharp
/// <summary>
/// Submission Review extension. When present, the platform uses
/// <c>SubmissionReviewExtension.Url</c> for LtiSubmissionReviewRequest
/// launches against this line item.
/// </summary>
[JsonPropertyName("submissionReview")]
public SubmissionReviewProperty SubmissionReviewExtension { get; set; }
```

Note the property is named `SubmissionReviewExtension` (not `SubmissionReview`) to avoid colliding with the `SubmissionReview` namespace introduced in Phase A. Update the test in Step 1 to reference `SubmissionReviewExtension` instead of `SubmissionReview`.

- [ ] **Step 5: Run; verify pass**

Run: `dotnet test test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj --filter FullyQualifiedName~LineItemShould`
Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/LtiAdvantage/AssignmentGradeServices/LineItem.cs src/LtiAdvantage/SubmissionReview/SubmissionReviewProperty.cs test/LtiAdvantage.UnitTests/AssignmentGradeServices/LineItemShould.cs
git commit -m "feat(sr): expose submissionReview extension on AGS LineItem"
```

### Task A5: Phase A regression sweep

- [ ] **Step 1: Run the full test suite**

Run: `dotnet test`
Expected: All tests pass on both `net8.0` and `net10.0`.

- [ ] **Step 2: Tag the phase**

```bash
git tag phase-a-submission-review
```

---

# Phase B — JWKS Publishing Endpoint

### Task B1: Define `IJwksKeyStore` abstraction

**Files:**
- Create: `src/LtiAdvantage/Jwks/IJwksKeyStore.cs`
- Modify: `src/LtiAdvantage/Constants.cs`

- [ ] **Step 1: Add the endpoint constant**

In `Constants.cs`, inside `class ServiceEndpoints`, add a new nested class (alphabetical, after `Ags`):

```csharp
/// <summary>
/// JWKS publication endpoint.
/// </summary>
public static class Jwks
{
    /// <summary>The well-known JWKS endpoint route name.</summary>
    public const string JwksService = "jwks";
}
```

In `Constants.cs`, inside `class MediaTypes`, add:

```csharp
/// <summary>
/// JSON Web Key Set media type (RFC 7517 §6).
/// </summary>
public const string Jwks = "application/jwk-set+json";
```

- [ ] **Step 2: Create the key store interface**

Create `src/LtiAdvantage/Jwks/IJwksKeyStore.cs`:

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.Jwks
{
    /// <summary>
    /// Provides the set of public keys to publish at /.well-known/jwks.json.
    /// Implementations decide how keys are stored, rotated, and retired —
    /// typically all currently-active and recently-retired (still in token TTL)
    /// signing keys are returned, each with a stable <c>kid</c>.
    /// </summary>
    public interface IJwksKeyStore
    {
        /// <summary>
        /// Returns the public keys that should appear in the JWKS document.
        /// Each key MUST have <c>Kid</c> set; <c>Use</c> SHOULD be "sig"; <c>Alg</c> SHOULD be "RS256".
        /// </summary>
        Task<IReadOnlyList<JsonWebKey>> GetPublicKeysAsync();
    }
}
```

- [ ] **Step 3: Build**

Run: `dotnet build src/LtiAdvantage/LtiAdvantage.csproj`
Expected: Build succeeded. (Note: `Microsoft.IdentityModel.Tokens.JsonWebKey` is available transitively via `System.IdentityModel.Tokens.Jwt 8.14.0` already referenced.)

- [ ] **Step 4: Commit**

```bash
git add src/LtiAdvantage/Constants.cs src/LtiAdvantage/Jwks/IJwksKeyStore.cs
git commit -m "feat(jwks): add IJwksKeyStore abstraction and Jwks endpoint constants"
```

### Task B2: `JwksControllerBase` with failing test

**Files:**
- Create: `src/LtiAdvantage.AspNetCore/Jwks/IJwksController.cs`
- Create: `src/LtiAdvantage.AspNetCore/Jwks/JwksControllerBase.cs`
- Create: `test/LtiAdvantage.IntegrationTests/Controllers/JwksController.cs`
- Create: `test/LtiAdvantage.IntegrationTests/Jwks/JwksControllerShould.cs`

- [ ] **Step 1: Write the failing integration test**

Create `test/LtiAdvantage.IntegrationTests/Jwks/JwksControllerShould.cs`:

```csharp
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LtiAdvantage.IntegrationTests.Jwks
{
    public class JwksControllerShould : IDisposable
    {
        private readonly HttpClient _client;
        private readonly TestServer _server;

        public JwksControllerShould()
        {
            _server = new TestServer(new WebHostBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureLogging(l => { l.AddConsole(); l.AddDebug(); })
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task ReturnPublicJwks_Anonymously()
        {
            var response = await _client.GetAsync(".well-known/jwks.json");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.StartsWith("application/jwk-set+json",
                response.Content.Headers.ContentType.ToString());

            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            Assert.True(doc.RootElement.TryGetProperty("keys", out var keys));
            Assert.True(keys.GetArrayLength() >= 1);

            var first = keys[0];
            Assert.Equal("test-kid-1", first.GetProperty("kid").GetString());
            Assert.Equal("sig", first.GetProperty("use").GetString());
            Assert.Equal("RSA", first.GetProperty("kty").GetString());
        }

        public void Dispose() { _client?.Dispose(); _server?.Dispose(); }
    }
}
```

- [ ] **Step 2: Run; verify fail**

Run: `dotnet test test/LtiAdvantage.IntegrationTests/LtiAdvantage.IntegrationTests.csproj --filter FullyQualifiedName~JwksControllerShould`
Expected: FAIL — 404 (no jwks endpoint yet).

- [ ] **Step 3: Define `IJwksController`**

Create `src/LtiAdvantage.AspNetCore/Jwks/IJwksController.cs`:

```csharp
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.AspNetCore.Jwks
{
    /// <summary>JWKS publication endpoint.</summary>
    public interface IJwksController
    {
        /// <summary>Returns the JSON Web Key Set.</summary>
        Task<ActionResult<JsonWebKeySet>> GetJwksAsync();
    }
}
```

- [ ] **Step 4: Implement `JwksControllerBase`**

Create `src/LtiAdvantage.AspNetCore/Jwks/JwksControllerBase.cs`:

```csharp
using System.Threading.Tasks;
using LtiAdvantage.Jwks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.AspNetCore.Jwks
{
    /// <summary>
    /// Publishes the platform's (or tool's) JWKS at a well-known URL so that
    /// peers can verify signed JWTs. Anonymous; unauthenticated.
    /// </summary>
    [ApiController]
    public abstract class JwksControllerBase : ControllerBase, IJwksController
    {
        private readonly IJwksKeyStore _keyStore;
        private readonly ILogger<JwksControllerBase> _logger;

        protected JwksControllerBase(IJwksKeyStore keyStore, ILogger<JwksControllerBase> logger)
        {
            _keyStore = keyStore;
            _logger = logger;
        }

        [HttpGet]
        [Produces(Constants.MediaTypes.Jwks)]
        [ProducesResponseType(typeof(JsonWebKeySet), StatusCodes.Status200OK)]
        [Route(".well-known/jwks.json", Name = Constants.ServiceEndpoints.Jwks.JwksService)]
        public async Task<ActionResult<JsonWebKeySet>> GetJwksAsync()
        {
            var keys = await _keyStore.GetPublicKeysAsync().ConfigureAwait(false);
            var set = new JsonWebKeySet();
            foreach (var k in keys) set.Keys.Add(k);
            return set;
        }
    }
}
```

- [ ] **Step 5: Stub controller for the test host**

Create `test/LtiAdvantage.IntegrationTests/Controllers/JwksController.cs`:

```csharp
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.Jwks;
using LtiAdvantage.Jwks;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class JwksController : JwksControllerBase
    {
        public JwksController(IJwksKeyStore keyStore, ILogger<JwksControllerBase> logger)
            : base(keyStore, logger) { }
    }

    public class TestJwksKeyStore : IJwksKeyStore
    {
        public Task<IReadOnlyList<JsonWebKey>> GetPublicKeysAsync()
        {
            using var rsa = RSA.Create(2048);
            var rsaParams = rsa.ExportParameters(includePrivateParameters: false);
            var jwk = new JsonWebKey
            {
                Kty = "RSA",
                Use = "sig",
                Alg = "RS256",
                Kid = "test-kid-1",
                N = Base64UrlEncoder.Encode(rsaParams.Modulus),
                E = Base64UrlEncoder.Encode(rsaParams.Exponent),
            };
            return Task.FromResult<IReadOnlyList<JsonWebKey>>(new[] { jwk });
        }
    }
}
```

- [ ] **Step 6: Register the key store in `Startup`**

Modify `test/LtiAdvantage.IntegrationTests/Startup.cs`. In `ConfigureServices`, after `services.AddLtiAdvantagePolicies();` add:

```csharp
services.AddSingleton<LtiAdvantage.Jwks.IJwksKeyStore, Controllers.TestJwksKeyStore>();
```

In `Configure`, replace the existing `endpoints.MapControllerRoute(...)` with both an attribute-route call and a fallback so attribute-routed `[Route(".well-known/jwks.json")]` works:

```csharp
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}");
});
```

- [ ] **Step 7: Run the test; verify pass**

Run: `dotnet test test/LtiAdvantage.IntegrationTests/LtiAdvantage.IntegrationTests.csproj --filter FullyQualifiedName~JwksControllerShould`
Expected: PASS.

- [ ] **Step 8: Run the full suite to make sure NRPS/AGS routes still resolve**

Run: `dotnet test`
Expected: All previously-passing tests still pass.

- [ ] **Step 9: Commit**

```bash
git add src/LtiAdvantage.AspNetCore/Jwks test/LtiAdvantage.IntegrationTests/Jwks test/LtiAdvantage.IntegrationTests/Controllers/JwksController.cs test/LtiAdvantage.IntegrationTests/Startup.cs
git commit -m "feat(jwks): add JwksControllerBase publishing public keys at /.well-known/jwks.json"
```

### Task B3: Document key-rotation pattern in XML doc

**Files:**
- Modify: `src/LtiAdvantage/Jwks/IJwksKeyStore.cs`

- [ ] **Step 1: Expand the interface XML doc**

Replace the interface body's XML comment with guidance the consumer needs to do rotation correctly:

```csharp
/// <summary>
/// Returns the public keys to publish in the JWKS document.
///
/// Rotation guidance:
/// - Always include the current signing key (the one used to sign tokens issued now).
/// - Continue to include each retired signing key for at least the longest TTL of any token signed with it,
///   so verifiers can still validate in-flight tokens after rotation.
/// - Each key MUST have a stable, unique <c>Kid</c>.
/// - Set <c>Use = "sig"</c> and <c>Alg = "RS256"</c> for LTI 1.3 signing keys.
/// - Do NOT include private key material — only the public components (<c>N</c>, <c>E</c>).
/// </summary>
Task<IReadOnlyList<JsonWebKey>> GetPublicKeysAsync();
```

- [ ] **Step 2: Build**

Run: `dotnet build`
Expected: Build succeeded.

- [ ] **Step 3: Commit**

```bash
git add src/LtiAdvantage/Jwks/IJwksKeyStore.cs
git commit -m "docs(jwks): document key-rotation contract on IJwksKeyStore"
```

### Task B4: Phase B regression sweep + tag

- [ ] **Step 1: Full suite**

Run: `dotnet test`
Expected: All tests pass on both `net8.0` and `net10.0`.

- [ ] **Step 2: Tag**

```bash
git tag phase-b-jwks
```

---

# Phase C — LTI Dynamic Registration 1.0

This is the largest phase. It has two halves: **C-models** (shared POCOs), **C-tool** (the tool-side handshake helper), **C-platform** (the platform-side registration controller).

### Task C1: Add Dynamic Registration constants

**Files:**
- Modify: `src/LtiAdvantage/Constants.cs`

- [ ] **Step 1: Add claim URIs and message type**

Inside `class LtiClaims`, add (alphabetical):

```csharp
/// <summary>The LTI Platform Configuration claim on an OpenID configuration document (Dynamic Registration §3.5).</summary>
public const string LtiPlatformConfiguration = "https://purl.imsglobal.org/spec/lti-platform-configuration";

/// <summary>The LTI Tool Configuration claim on a registration request body (Dynamic Registration §3.6).</summary>
public const string LtiToolConfiguration = "https://purl.imsglobal.org/spec/lti-tool-configuration";
```

Inside `class LtiScopes`, add a new nested class:

```csharp
/// <summary>LTI Dynamic Registration scopes (Dynamic Registration §4.4).</summary>
public static class DynamicRegistration
{
    /// <summary>Scope required on the registration access token.</summary>
    public const string Scope = "https://purl.imsglobal.org/spec/lti-reg/scope/registration";
}
```

- [ ] **Step 2: Build**

Run: `dotnet build src/LtiAdvantage/LtiAdvantage.csproj`
Expected: Build succeeded.

- [ ] **Step 3: Commit**

```bash
git add src/LtiAdvantage/Constants.cs
git commit -m "feat(dynreg): add Dynamic Registration claim and scope constants"
```

### Task C2: `PlatformOpenIdConfiguration` model

The platform's OIDC discovery doc (RFC 8414) PLUS LTI's `lti-platform-configuration` claim.

**Files:**
- Create: `src/LtiAdvantage/DynamicRegistration/PlatformOpenIdConfiguration.cs`
- Create: `src/LtiAdvantage/DynamicRegistration/LtiPlatformConfiguration.cs`
- Create: `test/LtiAdvantage.UnitTests/ReferenceJson/PlatformOpenIdConfiguration.json`
- Create: `test/LtiAdvantage.UnitTests/DynamicRegistration/PlatformOpenIdConfigurationShould.cs`
- Modify: `test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj`

- [ ] **Step 1: Reference JSON (sample taken from spec §3.5 example)**

Create `test/LtiAdvantage.UnitTests/ReferenceJson/PlatformOpenIdConfiguration.json`:

```json
{
  "issuer": "https://platform.example.com",
  "authorization_endpoint": "https://platform.example.com/auth",
  "token_endpoint": "https://platform.example.com/token",
  "token_endpoint_auth_methods_supported": ["private_key_jwt"],
  "token_endpoint_auth_signing_alg_values_supported": ["RS256"],
  "jwks_uri": "https://platform.example.com/.well-known/jwks.json",
  "registration_endpoint": "https://platform.example.com/lti/register",
  "scopes_supported": [
    "openid",
    "https://purl.imsglobal.org/spec/lti-ags/scope/lineitem",
    "https://purl.imsglobal.org/spec/lti-ags/scope/score",
    "https://purl.imsglobal.org/spec/lti-nrps/scope/contextmembership.readonly"
  ],
  "response_types_supported": ["id_token"],
  "subject_types_supported": ["public"],
  "id_token_signing_alg_values_supported": ["RS256"],
  "claims_supported": ["sub", "iss", "name", "email"],
  "https://purl.imsglobal.org/spec/lti-platform-configuration": {
    "product_family_code": "ExamplePlatform",
    "version": "1.0",
    "messages_supported": [
      { "type": "LtiResourceLinkRequest" },
      { "type": "LtiDeepLinkingRequest", "placements": ["ContentArea", "RichTextEditor"] }
    ],
    "variables": ["CourseSection.sourcedId", "Person.email.primary"]
  }
}
```

Register the file in `LtiAdvantage.UnitTests.csproj` exactly the way `LtiSubmissionReviewRequest.json` was registered in Task A3.

- [ ] **Step 2: Failing test**

Create `test/LtiAdvantage.UnitTests/DynamicRegistration/PlatformOpenIdConfigurationShould.cs`:

```csharp
using System.Text.Json;
using LtiAdvantage.DynamicRegistration;
using Xunit;

namespace LtiAdvantage.UnitTests.DynamicRegistration
{
    public class PlatformOpenIdConfigurationShould
    {
        [Fact]
        public void ParseSpecExample()
        {
            var json = TestUtils.LoadReferenceJsonFile("PlatformOpenIdConfiguration");
            var config = JsonSerializer.Deserialize<PlatformOpenIdConfiguration>(json);

            Assert.Equal("https://platform.example.com", config.Issuer);
            Assert.Equal("https://platform.example.com/lti/register", config.RegistrationEndpoint);
            Assert.Contains("private_key_jwt", config.TokenEndpointAuthMethodsSupported);
            Assert.NotNull(config.LtiPlatformConfiguration);
            Assert.Equal("ExamplePlatform", config.LtiPlatformConfiguration.ProductFamilyCode);
            Assert.Equal(2, config.LtiPlatformConfiguration.MessagesSupported.Count);
        }

        [Fact]
        public void RoundTrip()
        {
            var json = TestUtils.LoadReferenceJsonFile("PlatformOpenIdConfiguration");
            var config = JsonSerializer.Deserialize<PlatformOpenIdConfiguration>(json);
            var roundTripped = JsonSerializer.Serialize(config);
            JsonAssert.Equal(json, roundTripped);
        }
    }
}
```

Run: `dotnet test test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj --filter FullyQualifiedName~PlatformOpenIdConfigurationShould`
Expected: FAIL (compile error).

- [ ] **Step 3: Implement `LtiPlatformConfiguration`**

Create `src/LtiAdvantage/DynamicRegistration/LtiPlatformConfiguration.cs`:

```csharp
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// Value of the LTI extension claim on a platform's OpenID configuration document.
    /// See https://www.imsglobal.org/spec/lti-dr/v1p0/#lti-platform-configuration
    /// </summary>
    public class LtiPlatformConfiguration
    {
        [JsonPropertyName("product_family_code")]
        public string ProductFamilyCode { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>The set of LTI message types this platform supports launching, plus optional placements.</summary>
        [JsonPropertyName("messages_supported")]
        public IList<MessageDescriptor> MessagesSupported { get; set; }

        /// <summary>Custom variable expansions the platform supports (e.g. <c>"Person.email.primary"</c>).</summary>
        [JsonPropertyName("variables")]
        public IList<string> Variables { get; set; }
    }
}
```

Create `src/LtiAdvantage/DynamicRegistration/MessageDescriptor.cs`:

```csharp
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// Describes a supported LTI message type. Used in both
    /// <see cref="LtiPlatformConfiguration"/> (what the platform launches)
    /// and <see cref="LtiToolConfiguration"/> (what the tool can receive).
    /// </summary>
    public class MessageDescriptor
    {
        /// <summary>The message type, e.g. <c>"LtiResourceLinkRequest"</c>.</summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>Per-message target_link_uri (tool side).</summary>
        [JsonPropertyName("target_link_uri")]
        public string TargetLinkUri { get; set; }

        /// <summary>Per-message label shown by the platform (tool side).</summary>
        [JsonPropertyName("label")]
        public string Label { get; set; }

        /// <summary>Per-message icon URI (tool side).</summary>
        [JsonPropertyName("icon_uri")]
        public string IconUri { get; set; }

        /// <summary>Custom parameters to merge into this launch (tool side).</summary>
        [JsonPropertyName("custom_parameters")]
        public IDictionary<string, string> CustomParameters { get; set; }

        /// <summary>Placements this message is offered for (e.g. <c>"ContentArea"</c>, <c>"RichTextEditor"</c>).</summary>
        [JsonPropertyName("placements")]
        public IList<string> Placements { get; set; }

        /// <summary>Roles required for this launch (tool side).</summary>
        [JsonPropertyName("roles")]
        public IList<string> Roles { get; set; }
    }
}
```

- [ ] **Step 4: Implement `PlatformOpenIdConfiguration`**

Create `src/LtiAdvantage/DynamicRegistration/PlatformOpenIdConfiguration.cs`:

```csharp
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// A platform's OpenID Provider Metadata document (RFC 8414) including
    /// the LTI Dynamic Registration extension claim
    /// (<c>https://purl.imsglobal.org/spec/lti-platform-configuration</c>).
    /// </summary>
    public class PlatformOpenIdConfiguration
    {
        [JsonPropertyName("issuer")]
        public string Issuer { get; set; }

        [JsonPropertyName("authorization_endpoint")]
        public string AuthorizationEndpoint { get; set; }

        [JsonPropertyName("token_endpoint")]
        public string TokenEndpoint { get; set; }

        [JsonPropertyName("token_endpoint_auth_methods_supported")]
        public IList<string> TokenEndpointAuthMethodsSupported { get; set; }

        [JsonPropertyName("token_endpoint_auth_signing_alg_values_supported")]
        public IList<string> TokenEndpointAuthSigningAlgValuesSupported { get; set; }

        [JsonPropertyName("jwks_uri")]
        public string JwksUri { get; set; }

        [JsonPropertyName("registration_endpoint")]
        public string RegistrationEndpoint { get; set; }

        [JsonPropertyName("scopes_supported")]
        public IList<string> ScopesSupported { get; set; }

        [JsonPropertyName("response_types_supported")]
        public IList<string> ResponseTypesSupported { get; set; }

        [JsonPropertyName("subject_types_supported")]
        public IList<string> SubjectTypesSupported { get; set; }

        [JsonPropertyName("id_token_signing_alg_values_supported")]
        public IList<string> IdTokenSigningAlgValuesSupported { get; set; }

        [JsonPropertyName("claims_supported")]
        public IList<string> ClaimsSupported { get; set; }

        /// <summary>The LTI extension claim. Constant: <see cref="Constants.LtiClaims.LtiPlatformConfiguration"/>.</summary>
        [JsonPropertyName("https://purl.imsglobal.org/spec/lti-platform-configuration")]
        public LtiPlatformConfiguration LtiPlatformConfiguration { get; set; }
    }
}
```

- [ ] **Step 5: Run; verify pass**

Run: `dotnet test test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj --filter FullyQualifiedName~PlatformOpenIdConfigurationShould`
Expected: PASS (2 tests).

- [ ] **Step 6: Commit**

```bash
git add src/LtiAdvantage/DynamicRegistration test/LtiAdvantage.UnitTests/DynamicRegistration test/LtiAdvantage.UnitTests/ReferenceJson/PlatformOpenIdConfiguration.json test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj
git commit -m "feat(dynreg): add PlatformOpenIdConfiguration and supporting models"
```

### Task C3: `ToolConfiguration` model

The body the tool POSTs to the platform's `registration_endpoint`. Mirrors OIDC client metadata + the LTI tool-configuration claim.

**Files:**
- Create: `src/LtiAdvantage/DynamicRegistration/ToolConfiguration.cs`
- Create: `src/LtiAdvantage/DynamicRegistration/LtiToolConfiguration.cs`
- Create: `test/LtiAdvantage.UnitTests/ReferenceJson/ToolConfiguration.json`
- Create: `test/LtiAdvantage.UnitTests/DynamicRegistration/ToolConfigurationShould.cs`
- Modify: `test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj`

- [ ] **Step 1: Reference JSON (from spec §3.6)**

Create `test/LtiAdvantage.UnitTests/ReferenceJson/ToolConfiguration.json`:

```json
{
  "application_type": "web",
  "response_types": ["id_token"],
  "grant_types": ["implicit", "client_credentials"],
  "initiate_login_uri": "https://tool.example.com/login",
  "redirect_uris": ["https://tool.example.com/launch"],
  "client_name": "Example Tool",
  "client_uri": "https://tool.example.com",
  "logo_uri": "https://tool.example.com/logo.png",
  "scope": "https://purl.imsglobal.org/spec/lti-ags/scope/lineitem https://purl.imsglobal.org/spec/lti-ags/scope/score https://purl.imsglobal.org/spec/lti-nrps/scope/contextmembership.readonly",
  "token_endpoint_auth_method": "private_key_jwt",
  "jwks_uri": "https://tool.example.com/.well-known/jwks.json",
  "https://purl.imsglobal.org/spec/lti-tool-configuration": {
    "domain": "tool.example.com",
    "description": "An example LTI 1.3 tool",
    "target_link_uri": "https://tool.example.com/launch",
    "custom_parameters": { "context_history": "$Context.id.history" },
    "claims": ["iss", "sub", "name", "email"],
    "messages": [
      {
        "type": "LtiDeepLinkingRequest",
        "target_link_uri": "https://tool.example.com/dl",
        "label": "Add example content",
        "placements": ["ContentArea"]
      }
    ]
  }
}
```

Register this file in the csproj in the same pattern.

- [ ] **Step 2: Failing test**

Create `test/LtiAdvantage.UnitTests/DynamicRegistration/ToolConfigurationShould.cs`:

```csharp
using System.Text.Json;
using LtiAdvantage.DynamicRegistration;
using Xunit;

namespace LtiAdvantage.UnitTests.DynamicRegistration
{
    public class ToolConfigurationShould
    {
        [Fact]
        public void ParseSpecExample()
        {
            var json = TestUtils.LoadReferenceJsonFile("ToolConfiguration");
            var tool = JsonSerializer.Deserialize<ToolConfiguration>(json);

            Assert.Equal("web", tool.ApplicationType);
            Assert.Equal("private_key_jwt", tool.TokenEndpointAuthMethod);
            Assert.NotNull(tool.LtiToolConfiguration);
            Assert.Equal("tool.example.com", tool.LtiToolConfiguration.Domain);
            Assert.Single(tool.LtiToolConfiguration.Messages);
            Assert.Equal("LtiDeepLinkingRequest", tool.LtiToolConfiguration.Messages[0].Type);
        }

        [Fact]
        public void RoundTrip()
        {
            var json = TestUtils.LoadReferenceJsonFile("ToolConfiguration");
            var tool = JsonSerializer.Deserialize<ToolConfiguration>(json);
            var roundTripped = JsonSerializer.Serialize(tool);
            JsonAssert.Equal(json, roundTripped);
        }
    }
}
```

Run; expected: FAIL (compile).

- [ ] **Step 3: Implement `LtiToolConfiguration`**

Create `src/LtiAdvantage/DynamicRegistration/LtiToolConfiguration.cs`:

```csharp
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// LTI extension claim on a Dynamic Registration request body.
    /// See https://www.imsglobal.org/spec/lti-dr/v1p0/#lti-tool-configuration
    /// </summary>
    public class LtiToolConfiguration
    {
        [JsonPropertyName("domain")]
        public string Domain { get; set; }

        [JsonPropertyName("secondary_domains")]
        public IList<string> SecondaryDomains { get; set; }

        [JsonPropertyName("deployment_id")]
        public string DeploymentId { get; set; }

        [JsonPropertyName("target_link_uri")]
        public string TargetLinkUri { get; set; }

        [JsonPropertyName("custom_parameters")]
        public IDictionary<string, string> CustomParameters { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>The OpenID claims this tool requests (e.g. <c>"name"</c>, <c>"email"</c>).</summary>
        [JsonPropertyName("claims")]
        public IList<string> Claims { get; set; }

        /// <summary>The LTI message types this tool supports receiving.</summary>
        [JsonPropertyName("messages")]
        public IList<MessageDescriptor> Messages { get; set; }
    }
}
```

- [ ] **Step 4: Implement `ToolConfiguration`**

Create `src/LtiAdvantage/DynamicRegistration/ToolConfiguration.cs`:

```csharp
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DynamicRegistration
{
    /// <summary>
    /// Tool-side registration request body sent to a platform's
    /// <c>registration_endpoint</c>. Mirrors OIDC client metadata
    /// (RFC 7591) plus the LTI <c>lti-tool-configuration</c> claim.
    /// </summary>
    public class ToolConfiguration
    {
        [JsonPropertyName("application_type")]
        public string ApplicationType { get; set; } = "web";

        [JsonPropertyName("response_types")]
        public IList<string> ResponseTypes { get; set; }

        [JsonPropertyName("grant_types")]
        public IList<string> GrantTypes { get; set; }

        [JsonPropertyName("initiate_login_uri")]
        public string InitiateLoginUri { get; set; }

        [JsonPropertyName("redirect_uris")]
        public IList<string> RedirectUris { get; set; }

        [JsonPropertyName("client_name")]
        public string ClientName { get; set; }

        [JsonPropertyName("client_uri")]
        public string ClientUri { get; set; }

        [JsonPropertyName("logo_uri")]
        public string LogoUri { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("token_endpoint_auth_method")]
        public string TokenEndpointAuthMethod { get; set; } = "private_key_jwt";

        [JsonPropertyName("jwks_uri")]
        public string JwksUri { get; set; }

        /// <summary>Echoed back by the platform (Dynamic Registration §4.5).</summary>
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("https://purl.imsglobal.org/spec/lti-tool-configuration")]
        public LtiToolConfiguration LtiToolConfiguration { get; set; }
    }
}
```

- [ ] **Step 5: Run; verify pass**

Run: `dotnet test test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj --filter FullyQualifiedName~ToolConfigurationShould`
Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/LtiAdvantage/DynamicRegistration/ToolConfiguration.cs src/LtiAdvantage/DynamicRegistration/LtiToolConfiguration.cs test/LtiAdvantage.UnitTests/DynamicRegistration/ToolConfigurationShould.cs test/LtiAdvantage.UnitTests/ReferenceJson/ToolConfiguration.json test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj
git commit -m "feat(dynreg): add ToolConfiguration registration request model"
```

### Task C4: Tool-side handshake helper (`HttpClientDynamicRegistrationExtensions`)

Lets a tool fetch the platform OIDC discovery doc and post a registration request with one call.

**Files:**
- Create: `src/LtiAdvantage.IdentityModel/Client/HttpClientDynamicRegistrationExtensions.cs`
- Create: `test/LtiAdvantage.UnitTests/DynamicRegistration/HttpClientDynamicRegistrationExtensionsShould.cs`

- [ ] **Step 1: Write the failing test**

Create the test file. It uses a mock `HttpMessageHandler` so no real network is involved. Use a small inline handler — the repo already pulls `Microsoft.NET.Test.Sdk`, no extra packages needed.

```csharp
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using LtiAdvantage.IdentityModel.Client;
using Xunit;

namespace LtiAdvantage.UnitTests.DynamicRegistration
{
    public class HttpClientDynamicRegistrationExtensionsShould
    {
        [Fact]
        public async Task FetchPlatformConfiguration_FromOpenidConfigurationUrl()
        {
            var configJson = TestUtils.LoadReferenceJsonFile("PlatformOpenIdConfiguration");
            var handler = new StubHandler((req, ct) =>
            {
                Assert.Equal("https://platform.example.com/.well-known/openid-configuration",
                    req.RequestUri.ToString());
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(configJson, Encoding.UTF8, "application/json")
                };
            });
            using var client = new HttpClient(handler);

            var config = await client.GetPlatformOpenIdConfigurationAsync(
                "https://platform.example.com/.well-known/openid-configuration");

            Assert.Equal("https://platform.example.com", config.Issuer);
            Assert.Equal("https://platform.example.com/lti/register", config.RegistrationEndpoint);
        }

        [Fact]
        public async Task PostRegistration_WithBearerToken()
        {
            var responseJson = TestUtils.LoadReferenceJsonFile("ToolConfiguration");
            // Pretend the platform echoed back with a client_id assigned.
            var responseTool = JsonSerializer.Deserialize<ToolConfiguration>(responseJson);
            responseTool.ClientId = "client-9";
            var responseBody = JsonSerializer.Serialize(responseTool);

            HttpRequestMessage capturedRequest = null;
            var handler = new StubHandler((req, ct) =>
            {
                capturedRequest = req;
                return new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
                };
            });
            using var client = new HttpClient(handler);

            var request = JsonSerializer.Deserialize<ToolConfiguration>(
                TestUtils.LoadReferenceJsonFile("ToolConfiguration"));

            var result = await client.RegisterToolAsync(
                "https://platform.example.com/lti/register",
                "registration-token-xyz",
                request);

            Assert.Equal("client-9", result.ClientId);
            Assert.Equal("Bearer", capturedRequest.Headers.Authorization.Scheme);
            Assert.Equal("registration-token-xyz", capturedRequest.Headers.Authorization.Parameter);
            Assert.Equal("application/json", capturedRequest.Content.Headers.ContentType.MediaType);
        }

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> _fn;
            public StubHandler(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> fn) => _fn = fn;
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage r, CancellationToken c)
                => Task.FromResult(_fn(r, c));
        }
    }
}
```

Run: `dotnet test test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj --filter FullyQualifiedName~HttpClientDynamicRegistrationExtensionsShould`
Expected: FAIL (compile).

- [ ] **Step 2: Add a project reference if needed**

The unit-test project currently only references `LtiAdvantage`. The extensions live in `LtiAdvantage.IdentityModel`. Either: (a) add a project reference from unit tests to `LtiAdvantage.IdentityModel`, or (b) keep extensions accessible from `LtiAdvantage` only.

Choose (a). In `test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj`, add:

```xml
<ProjectReference Include="..\..\src\LtiAdvantage.IdentityModel\LtiAdvantage.IdentityModel.csproj" />
```

- [ ] **Step 3: Implement the extensions**

Create `src/LtiAdvantage.IdentityModel/Client/HttpClientDynamicRegistrationExtensions.cs`:

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;

namespace LtiAdvantage.IdentityModel.Client
{
    /// <summary>
    /// HttpClient extensions implementing the tool side of LTI Dynamic Registration 1.0.
    /// </summary>
    public static class HttpClientDynamicRegistrationExtensions
    {
        /// <summary>
        /// Fetches the platform's OpenID configuration document (<see cref="PlatformOpenIdConfiguration"/>).
        /// </summary>
        /// <param name="client">The HTTP client.</param>
        /// <param name="openIdConfigurationUrl">The URL passed by the platform in the registration init launch.</param>
        public static async Task<PlatformOpenIdConfiguration> GetPlatformOpenIdConfigurationAsync(
            this HttpClient client, string openIdConfigurationUrl, CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrWhiteSpace(openIdConfigurationUrl))
                throw new ArgumentException("URL is required", nameof(openIdConfigurationUrl));

            using var response = await client.GetAsync(openIdConfigurationUrl, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<PlatformOpenIdConfiguration>(body);
        }

        /// <summary>
        /// POSTs a tool registration request to the platform's <c>registration_endpoint</c>
        /// using the bearer registration token supplied during the registration init launch.
        /// Returns the platform-assigned <see cref="ToolConfiguration"/> (echoed config + <c>client_id</c>).
        /// </summary>
        public static async Task<ToolConfiguration> RegisterToolAsync(
            this HttpClient client,
            string registrationEndpoint,
            string registrationAccessToken,
            ToolConfiguration tool,
            CancellationToken cancellationToken = default)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrWhiteSpace(registrationEndpoint))
                throw new ArgumentException("URL is required", nameof(registrationEndpoint));
            if (string.IsNullOrWhiteSpace(registrationAccessToken))
                throw new ArgumentException("Token is required", nameof(registrationAccessToken));
            if (tool == null) throw new ArgumentNullException(nameof(tool));

            var json = JsonSerializer.Serialize(tool);
            using var request = new HttpRequestMessage(HttpMethod.Post, registrationEndpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", registrationAccessToken);

            using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<ToolConfiguration>(body);
        }
    }
}
```

Note: `LtiAdvantage.IdentityModel` already targets net8.0/net10.0 and references `LtiAdvantage`, so `using LtiAdvantage.DynamicRegistration` will resolve.

- [ ] **Step 4: Run; verify pass**

Run: `dotnet test test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj --filter FullyQualifiedName~HttpClientDynamicRegistrationExtensionsShould`
Expected: PASS (2 tests).

- [ ] **Step 5: Commit**

```bash
git add src/LtiAdvantage.IdentityModel/Client/HttpClientDynamicRegistrationExtensions.cs test/LtiAdvantage.UnitTests/DynamicRegistration/HttpClientDynamicRegistrationExtensionsShould.cs test/LtiAdvantage.UnitTests/LtiAdvantage.UnitTests.csproj
git commit -m "feat(dynreg): tool-side HttpClient extensions for OIDC discovery and registration"
```

### Task C5: Platform-side `DynamicRegistrationControllerBase`

The platform receives the POSTed `ToolConfiguration`, persists it, assigns a `client_id`, returns it.

**Files:**
- Create: `src/LtiAdvantage.AspNetCore/DynamicRegistration/IDynamicRegistrationController.cs`
- Create: `src/LtiAdvantage.AspNetCore/DynamicRegistration/RegisterToolRequest.cs`
- Create: `src/LtiAdvantage.AspNetCore/DynamicRegistration/DynamicRegistrationControllerBase.cs`
- Create: `test/LtiAdvantage.IntegrationTests/Controllers/DynamicRegistrationController.cs`
- Create: `test/LtiAdvantage.IntegrationTests/DynamicRegistration/DynamicRegistrationControllerShould.cs`

- [ ] **Step 1: Write the failing integration test**

Create `test/LtiAdvantage.IntegrationTests/DynamicRegistration/DynamicRegistrationControllerShould.cs`:

```csharp
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
            // No x-test-scope header → TestAuthHandler issues no scope claim.
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
```

Run: `dotnet test test/LtiAdvantage.IntegrationTests/LtiAdvantage.IntegrationTests.csproj --filter FullyQualifiedName~DynamicRegistrationControllerShould`
Expected: FAIL — 404 (no controller yet).

- [ ] **Step 2: Define the interface**

Create `src/LtiAdvantage.AspNetCore/DynamicRegistration/IDynamicRegistrationController.cs`:

```csharp
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantage.AspNetCore.DynamicRegistration
{
    /// <summary>The platform-side LTI Dynamic Registration endpoint.</summary>
    public interface IDynamicRegistrationController
    {
        /// <summary>Registers a new tool. Returns 201 with the assigned <c>client_id</c> echoed back.</summary>
        Task<ActionResult<ToolConfiguration>> RegisterAsync([FromBody] ToolConfiguration tool);
    }
}
```

- [ ] **Step 3: Add the request envelope**

Create `src/LtiAdvantage.AspNetCore/DynamicRegistration/RegisterToolRequest.cs`:

```csharp
using LtiAdvantage.DynamicRegistration;

namespace LtiAdvantage.AspNetCore.DynamicRegistration
{
    /// <summary>Request passed to the controller's <c>OnRegisterAsync</c> override.</summary>
    public class RegisterToolRequest
    {
        public RegisterToolRequest(ToolConfiguration tool) => Tool = tool;

        /// <summary>The submitted tool configuration. The override should populate <see cref="ToolConfiguration.ClientId"/>.</summary>
        public ToolConfiguration Tool { get; }
    }
}
```

- [ ] **Step 4: Implement the controller base**

Create `src/LtiAdvantage.AspNetCore/DynamicRegistration/DynamicRegistrationControllerBase.cs`:

```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.AspNetCore.DynamicRegistration
{
    /// <summary>
    /// Platform-side endpoint for LTI Dynamic Registration 1.0.
    /// See https://www.imsglobal.org/spec/lti-dr/v1p0/#registration-endpoint
    /// </summary>
    [ApiController]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class DynamicRegistrationControllerBase : ControllerBase, IDynamicRegistrationController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DynamicRegistrationControllerBase> _logger;

        protected DynamicRegistrationControllerBase(IWebHostEnvironment env, ILogger<DynamicRegistrationControllerBase> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Persist the registration. Implementations MUST set <see cref="ToolConfiguration.ClientId"/>
        /// on <c>request.Tool</c> (or return a new instance) before returning.
        /// </summary>
        protected abstract Task<ActionResult<ToolConfiguration>> OnRegisterAsync(RegisterToolRequest request);

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(ToolConfiguration), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = Constants.LtiScopes.DynamicRegistration.Scope)]
        [Route("lti/register", Name = "lti-dynamic-registration")]
        public async Task<ActionResult<ToolConfiguration>> RegisterAsync([Required] [FromBody] ToolConfiguration tool)
        {
            try
            {
                _logger.LogDebug($"Entering {nameof(RegisterAsync)}.");
                var result = await OnRegisterAsync(new RegisterToolRequest(tool)).ConfigureAwait(false);
                if (result.Result is ObjectResult o && o.StatusCode == null) o.StatusCode = StatusCodes.Status201Created;
                else if (result.Value != null && result.Result == null) return Created(string.Empty, result.Value);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An unexpected error occurred in {nameof(RegisterAsync)}.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "An unexpected error occurred",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = _env.IsDevelopment() ? ex.Message + ex.StackTrace : ex.Message
                });
            }
            finally
            {
                _logger.LogDebug($"Exiting {nameof(RegisterAsync)}.");
            }
        }
    }
}
```

- [ ] **Step 5: Stub controller in tests**

Create `test/LtiAdvantage.IntegrationTests/Controllers/DynamicRegistrationController.cs`:

```csharp
using System;
using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.DynamicRegistration;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class DynamicRegistrationController : DynamicRegistrationControllerBase
    {
        public DynamicRegistrationController(IWebHostEnvironment env, ILogger<DynamicRegistrationControllerBase> logger)
            : base(env, logger) { }

        protected override Task<ActionResult<ToolConfiguration>> OnRegisterAsync(RegisterToolRequest request)
        {
            request.Tool.ClientId = Guid.NewGuid().ToString();
            return Task.FromResult<ActionResult<ToolConfiguration>>(
                new ObjectResult(request.Tool) { StatusCode = 201 });
        }
    }
}
```

- [ ] **Step 6: Run; verify pass**

Run: `dotnet test test/LtiAdvantage.IntegrationTests/LtiAdvantage.IntegrationTests.csproj --filter FullyQualifiedName~DynamicRegistrationControllerShould`
Expected: PASS (2 tests).

- [ ] **Step 7: Commit**

```bash
git add src/LtiAdvantage.AspNetCore/DynamicRegistration test/LtiAdvantage.IntegrationTests/Controllers/DynamicRegistrationController.cs test/LtiAdvantage.IntegrationTests/DynamicRegistration
git commit -m "feat(dynreg): platform-side DynamicRegistrationControllerBase"
```

### Task C6: README pointers

**Files:**
- Modify: `README.md`

- [ ] **Step 1: Add a "What's new" section**

After the bullet list of libraries (above NuGet table), insert:

```markdown
### What's new

- LTI Submission Review 1.0 (`LtiSubmissionReviewRequest`, `for_user` claim, AGS `submissionReview` extension)
- JWKS publishing (`JwksControllerBase` + `IJwksKeyStore`) for `/.well-known/jwks.json`
- LTI Dynamic Registration 1.0 — tool-side `HttpClient` extensions and platform-side `DynamicRegistrationControllerBase`
```

- [ ] **Step 2: Commit**

```bash
git add README.md
git commit -m "docs: announce Submission Review, JWKS, Dynamic Registration"
```

### Task C7: Phase C regression sweep + tag

- [ ] **Step 1: Full suite**

Run: `dotnet test`
Expected: All tests pass on both `net8.0` and `net10.0`. If anything fails, do NOT proceed — fix root cause.

- [ ] **Step 2: Tag**

```bash
git tag phase-c-dynamic-registration
```

---

## Out-of-scope (intentional)

- Platform-side OIDC configuration discovery endpoint (the `/.well-known/openid-configuration` doc itself). Could be added later behind another `OpenIdConfigurationControllerBase`; not required for tools to register if the platform exposes it via a fixed CMS-style page.
- Persistent key store implementation. We ship the `IJwksKeyStore` interface only; consumers wire up their own (file, DB, KMS, etc.).
- The "registration init" launch (a redirect from the platform to the tool's `openid_configuration` URL with `?openid_configuration=…&registration_token=…`). Tools handle this in their own MVC/Razor layer; we don't wrap it.
- Asset Processor, Proctoring, Course Groups — see `2026-05-03-lti-roadmap-tier2-tier3.md`.
