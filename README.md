# LTI Advantage Libraries for .NET

- [LtiAdvantage](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage) is a library to ease the work of creating an LTI Advantage platform or tool.
- [LtiAdvantage.IdentityModel](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.IdentityModel) has an HttpClient extension method to request a token using JWT client credentials.
- [LtiAdvantage.Oidc](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.Oidc) extends an OpenIddict server with impersonation support (it replaces LtiAdvantage.IdentityServer4).
- [LtiAdvantage.AspNetCore](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.AspNetCore) contains ASP.NET Core controller implementations to assist in platform development.
- [LtiAdvantage.IntegrationTests](https://github.com/LtiLibrary/LtiAdvantage/tree/master/test/LtiAdvantage.IntegrationTests) integration tests.
- [LtiAdvantage.UnitTests](https://github.com/LtiLibrary/LtiAdvantage/tree/master/test/LtiAdvantage.UnitTests) unit tests.

### 4.2.0 (unreleased)

4.2.0 is binary compatible with 4.1.0: code built against 4.1.0 runs without recompiling.

#### Fixed and added

- `LtiDeepLinkingRequest` reads and writes the AGS, NRPS and Course Groups service claims. The AGS specification says the endpoint claim belongs in any LTI message whose context gives the tool access to the services, and a tool often creates line items while handling a deep linking request (#264).
- New shared bases: `LtiPlatformLaunchRequest` declares the service claims once for the resource link, submission review and deep linking requests, and `LtiProctoringRequest` declares `ResourceLink`, `AttemptNumber` and `ProctoringSettings` once for the three proctoring messages. Code that sets these claims can now work with the base type instead of switching on the message type (#264).
- The proctoring messages still carry no service claims: the Proctoring Services specification lists the optional claims for each message, and no service claim is among them (#264).

### 4.1.0

4.1.0 replaces 4.0.0, which is deprecated on NuGet. It fixes what building the samples against 4.0.0
turned up, and it has breaking changes, so code built against 4.0.0 must be recompiled.

#### Fixed and added

- AGS and NRPS controllers bind the spec's query parameter names (`resource_link_id`, `resource_id`, `user_id`) and accept the role URI in NRPS `role`; the old names still work (#301).
- Deep linking content items read and write `width`, `height`, `expiresAt` and the `embed` object by their spec names; `DocumentTarget.Embed`, and `gradesReleased` / `submissionReview` on a deep-linked `lineItem` (#302, #304).
- `Score.ScoreGiven` / `ScoreMaximum` are optional, and AGS models leave unset optional properties out (#303).
- The OpenID configuration document has no explicit nulls, and `MvcOptions.MapLtiRoute` moves any of the library's routes, for example away from OpenIddict's discovery endpoint (#305).
- Tool helpers: `InMemoryNonceStore`, a login-nonce check on `LtiTokenValidator`, per-registration JWKS, and `HttpClient` extensions for AGS, NRPS and Course Groups (#306).
- `RsaPemKeys` replaces `PemHelper` (sets `kid`, no BouncyCastle); `AccessTokenUtil` takes an `HttpClient` and writes a numeric `iat` (#307).
- `ToolPlatform` / `ToolPlatformInstance` replace the misspelled `ToolPlatfom*` names, and `Role.ToUri()` / `ContextType.ToUri()` are public (#308).
- `JwksCache` refetches a JWKS that was empty when first fetched, for example a tool registered before its key was published (#314).

#### Breaking changes

- `ImageProperty.Width` / `Height` are `int?` (#302), and `Score.ScoreGiven` / `ScoreMaximum` are `double?` (#303). Use `.Value` or `?? 0` where you read them, and leave the score values null for a progress-only score.
- `CustomPropertySubstitutions.ToolPlatform` / `ToolPlatformInstance` use the correctly spelled `ToolPlatform` / `ToolPlatformInstance` types; the misspelled names remain as obsolete aliases (#308).
- `LtiAdvantage.Lti.CourseGroup` is renamed to `LtiCourseGroup`, so it no longer clashes with an app's own `CourseGroup` (#320).
- `ForUserClaimValueType.Roles` is `Role[]`, like the launch's roles claim, instead of `IList<string>` (#321). Convert with `RoleExtensions.TryParse` and `Role.ToUri()`. A role the library does not define reads as `Role.Unknown`.

#### Changes on the wire

- Deep linking content items are written with `width`, `height`, `expiresAt` and `embed: {"html": …}` instead of `Width`, `Height`, `ExpiresAt` and a bare `embed` string. Both forms are still read (#302, #304).
- AGS line items, results and scores leave unset optional properties out instead of writing `null`, and a progress-only score has no `scoreGiven` / `scoreMaximum` (#303).
- The client assertion's `iat` is a number, and it carries the `kid` when you pass a key id (#307).
- Roles arrays (the roles claim, NRPS members, `for_user`) leave `Role.Unknown` out instead of writing `null` (#321).

#### Deprecated

- `PemHelper` (`LTIADV001`) and the `AccessTokenUtil.GetAccessTokenAsync` overload without an `HttpClient` (`LTIADV002`) still work but are obsolete; use `RsaPemKeys` and the overload that takes an `HttpClient` and key id (#307). The misspelled `ToolPlatfom*` names are obsolete too (#308).

### What's new in 4.0.0

- [LTI Course Groups Service 1.0](https://www.imsglobal.org/spec/lti-gs/v1p0) — `groupsservice` claim, `GroupsControllerBase` + `GroupSetsControllerBase`, and `group_enrollments` on the NRPS membership response (`?groups=true`)
- [LTI Proctoring Services 1.0](https://www.imsglobal.org/spec/proctoring/v1p0) — `LtiStartProctoringRequest`, `LtiStartAssessmentRequest`, `LtiEndAssessmentRequest`, and the Assessment Control Service (`AssessmentControlControllerBase`)
- `LtiTokenValidator` — validates an ID token's signature against the platform's JWKS (fetched and cached, refreshed on key rotation) plus `iss`/`aud`/`exp`/`nonce` and the LTI claims
- `OpenIdConfigurationControllerBase` for platform `/.well-known/openid-configuration`, and `InMemoryJwksKeyStore` as a ready-made `IJwksKeyStore`
- [LTI Submission Review 1.0](https://www.imsglobal.org/spec/lti-sr/v1p0) (`LtiSubmissionReviewRequest`, `for_user` claim, AGS `submissionReview` extension)
- [JWKS publishing](https://www.imsglobal.org/spec/security/v1p0#tool-jwk-set-url) (`JwksControllerBase` + `IJwksKeyStore`) for `/.well-known/jwks.json`
- [LTI Dynamic Registration 1.0](https://www.imsglobal.org/spec/lti-dr/v1p0) — tool-side `HttpClient` extensions and platform-side `DynamicRegistrationControllerBase`

#### Fixed

- Custom-variable substitution: `$Context.id.history` and `$ResourceLink.id.history` were mangled by the shorter `.id` variable being substituted first, and `$Context.sourcedId` was unreachable because the sourced id was written over `$Context.title`.
- The `…ControllerBase` classes no longer turn a client disconnect into a 500.

## XML documentation in your app

The packages ship XML documentation. To have it copied into your app's output (so Swagger or
another OpenAPI generator can show the controller and model docs), set this in your project file:

```xml
<PropertyGroup>
  <CopyDocumentationFilesFromPackages>true</CopyDocumentationFilesFromPackages>
</PropertyGroup>
```

## Samples

Two sample applications show the libraries in use:
- [Sample Tool](https://github.com/LtiLibrary/LtiAdvantageTool)
- [Sample Platform](https://github.com/LtiLibrary/LtiAdvantagePlatform)

## NuGet
| Library | Release | Prerelease |
| --- | --- | --- |
| LtiAdvantage | [![Nuget](https://img.shields.io/nuget/v/LtiAdvantage)](https://www.nuget.org/packages/LtiAdvantage) | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/LtiAdvantage)](https://www.nuget.org/packages/LtiAdvantage/absoluteLatest) |
| LtiAdvantage.IdentityModel | [![Nuget](https://img.shields.io/nuget/v/LtiAdvantage.IdentityModel)](https://www.nuget.org/packages/LtiAdvantage.IdentityModel) | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/LtiAdvantage.IdentityModel)](https://www.nuget.org/packages/LtiAdvantage.IdentityModel/absoluteLatest) |
| ~LtiAdvantage.IdentityServer4~ | [![Nuget](https://img.shields.io/nuget/v/LtiAdvantage.IdentityServer4)](https://www.nuget.org/packages/LtiAdvantage.IdentityServer4) |  [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/LtiAdvantage.IdentityServer4)](https://www.nuget.org/packages/LtiAdvantage.IdentityServer4/absoluteLatest) |
| LtiAdvantage.OIDC | [![Nuget](https://img.shields.io/nuget/v/LtiAdvantage.OIDC)](https://www.nuget.org/packages/LtiAdvantage.OIDC) |  [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/LtiAdvantage.OIDC)](https://www.nuget.org/packages/LtiAdvantage.OIDC/absoluteLatest) |
| LtiAdvantage.AspNetCore | [![Nuget](https://img.shields.io/nuget/v/LtiAdvantage.AspNetCore)](https://www.nuget.org/packages/LtiAdvantage.AspNetCore) |  [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/LtiAdvantage.IdentityServer4)](https://www.nuget.org/packages/LtiAdvantage.AspNetCore/absoluteLatest) |

## Build Status

| CI | Env | Status |
| --- | --- | --- |
| AppVeyor | Windows | [![Build status](https://ci.appveyor.com/api/projects/status/osmx09wp6le8ue03?svg=true)](https://ci.appveyor.com/project/andyfmiller/ltiadvantage) |
| travis-ci | Ubuntu | [![Build Status](https://travis-ci.org/andyfmiller/LtiAdvantage.svg?branch=master)](https://travis-ci.org/andyfmiller/LtiAdvantage) |

## Getting Started

There are two sample applications you can reference for ideas:
- [Sample Tool](https://github.com/LtiLibrary/LtiAdvantageTool)
- [Sample Platform](https://github.com/LtiLibrary/LtiAdvantagePlatform)
