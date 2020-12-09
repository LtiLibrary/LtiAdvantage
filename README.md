# LTI Advantage Libraries for .NET

- [LtiAdvantage](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage) is a library to ease the work of creating an LTI Advantage platform or tool.
- [LtiAdvantage.IdentityModel](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.IdentityModel) has an HttpClient extension method to request a token using JWT client credentials.
- [LtiAdvantage.IdentityServer4](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.IdentityServer4) has an Identity Server 4 secret validator that understands the [IMS recommended format](https://www.imsglobal.org/spec/security/v1p0#using-json-web-tokens-with-oauth-2-0-client-credentials-grant) of client-credentials grant.
- [LtiAdvantage.AspNetCore](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.AspNetCore) contains ASP.NET Core controller implementations to assist in platform development.
- [LtiAdvantage.IntegrationTests](https://github.com/LtiLibrary/LtiAdvantage/tree/master/test/LtiAdvantage.IntegrationTests) integration tests.
- [LtiAdvantage.UnitTests](https://github.com/LtiLibrary/LtiAdvantage/tree/master/test/LtiAdvantage.UnitTests) unit tests.

## NuGet
| Library | Release | Prerelease |
| --- | --- | --- |
| LtiAdvantage | [![Nuget](https://img.shields.io/nuget/v/LtiAdvantage)](https://www.nuget.org/packages/LtiAdvantage) | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/LtiAdvantage)](https://www.nuget.org/packages/LtiAdvantage/absoluteLatest) |
| LtiAdvantage.IdentityModel | [![Nuget](https://img.shields.io/nuget/v/LtiAdvantage.IdentityModel)](https://www.nuget.org/packages/LtiAdvantage.IdentityModel) | [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/LtiAdvantage.IdentityModel)](https://www.nuget.org/packages/LtiAdvantage.IdentityModel/absoluteLatest) |
| LtiAdvantage.IdentityServer4 | [![Nuget](https://img.shields.io/nuget/v/LtiAdvantage.IdentityServer4)](https://www.nuget.org/packages/LtiAdvantage.IdentityServer4) |  [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/LtiAdvantage.IdentityServer4)](https://www.nuget.org/packages/LtiAdvantage.IdentityServer4/absoluteLatest) |
| LtiAdvantage.AspNetCore | [![Nuget](https://img.shields.io/nuget/v/LtiAdvantage.AspNetCore)](https://www.nuget.org/packages/LtiAdvantage.AspNetCore) |  [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/LtiAdvantage.IdentityServer4)](https://www.nuget.org/packages/LtiAdvantage.AspNetCore/absoluteLatest) |

## Build Status

| CI | Env | Status |
| --- | --- | --- |
| AppVeyor | Windows | [![Build status](https://ci.appveyor.com/api/projects/status/osmx09wp6le8ue03?svg=true)](https://ci.appveyor.com/project/andyfmiller/ltiadvantage) |
| travis-ci | Ubuntu | [![Build Status](https://travis-ci.org/andyfmiller/LtiAdvantage.svg?branch=master)](https://travis-ci.org/andyfmiller/LtiAdvantage) |

## Getting Started

There are two sample applications you can reference for ideas:
- [Sample Tool](https://github.com/andyfmiller/LtiAdvantageTool)
- [Sample Platform](https://github.com/andyfmiller/LtiAdvantagePlatform)
