# LTI Advantage Libraries for .NET

- [LtiAdvantage](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage) is a library to ease the work of creating an LTI Advantage platform or tool.
- [LtiAdvantage.IdentityModel](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.IdentityModel) has an HttpClient extension method to request a token using JWT client credentials.
- [LtiAdvantage.Oidc](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.Oidc) extends an OpenIddict server with impersonation support (it replaces LtiAdvantage.IdentityServer4).
- [LtiAdvantage.AspNetCore](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.AspNetCore) contains ASP.NET Core controller implementations to assist in platform development.
- [LtiAdvantage.IntegrationTests](https://github.com/LtiLibrary/LtiAdvantage/tree/master/test/LtiAdvantage.IntegrationTests) integration tests.
- [LtiAdvantage.UnitTests](https://github.com/LtiLibrary/LtiAdvantage/tree/master/test/LtiAdvantage.UnitTests) unit tests.

### What's new

- [LTI Course Groups Service 1.0](https://www.imsglobal.org/spec/lti-gs/v1p0) — `groupsservice` claim, `GroupsControllerBase` + `GroupSetsControllerBase`, and `group_enrollments` on the NRPS membership response (`?groups=true`)
- [LTI Proctoring Services 1.0](https://www.imsglobal.org/spec/proctoring/v1p0) — `LtiStartProctoringRequest`, `LtiStartAssessmentRequest`, `LtiEndAssessmentRequest`, and the Assessment Control Service (`AssessmentControlControllerBase`)
- `LtiTokenValidator` — validates an ID token's signature against the platform's JWKS (fetched and cached, refreshed on key rotation) plus `iss`/`aud`/`exp`/`nonce` and the LTI claims
- `OpenIdConfigurationControllerBase` for platform `/.well-known/openid-configuration`, and `InMemoryJwksKeyStore` as a ready-made `IJwksKeyStore`
- [LTI Submission Review 1.0](https://www.imsglobal.org/spec/lti-sr/v1p0) (`LtiSubmissionReviewRequest`, `for_user` claim, AGS `submissionReview` extension)
- [JWKS publishing](https://www.imsglobal.org/spec/security/v1p0#tool-jwk-set-url) (`JwksControllerBase` + `IJwksKeyStore`) for `/.well-known/jwks.json`
- [LTI Dynamic Registration 1.0](https://www.imsglobal.org/spec/lti-dr/v1p0) — tool-side `HttpClient` extensions and platform-side `DynamicRegistrationControllerBase`

#### Fixed

- AGS and NRPS controllers bind the spec's query parameter names (`resource_link_id`, `resource_id`, `user_id`) and accept the role URI in NRPS `role`; the old names still work (#301).
- Deep linking content items read and write `width`, `height`, `expiresAt` and the `embed` object by their spec names; `DocumentTarget.Embed`, and `gradesReleased` / `submissionReview` on a deep-linked `lineItem` (#302, #304).
- `Score.ScoreGiven` / `ScoreMaximum` are optional, and AGS models leave unset optional properties out (#303).
- The OpenID configuration document has no explicit nulls, and `MvcOptions.MapLtiRoute` moves any of the library's routes, for example away from OpenIddict's discovery endpoint (#305).
- Tool helpers: `InMemoryNonceStore`, a login-nonce check on `LtiTokenValidator`, per-registration JWKS, and `HttpClient` extensions for AGS, NRPS and Course Groups (#306).
- `RsaPemKeys` replaces `PemHelper` (sets `kid`, no BouncyCastle); `AccessTokenUtil` takes an `HttpClient` and writes a numeric `iat` (#307).
- `ToolPlatform` / `ToolPlatformInstance` replace the misspelled `ToolPlatfom*` names, and `Role.ToUri()` / `ContextType.ToUri()` are public (#308).

**Source-breaking:** `ImageProperty.Width` / `Height` are now `int?` and `Score.ScoreGiven` / `ScoreMaximum` are now `double?`.

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
