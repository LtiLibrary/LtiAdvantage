# LtiAdvantage.IdentityServer4

LtiAdvantage.IdentityServer4 has an Identity Server 4 secret validator that understands the 
[IMS recommended format](https://www.imsglobal.org/spec/security/v1p0#using-json-web-tokens-with-oauth-2-0-client-credentials-grant)
of client-credentials grant. This is only useful if you use IdentityServer4 for client credential grants.

## ASP.NET Core

This library targets `netcoreapp2.1` and uses BouncyCastle.NetCore to read PEM formatted keys
for compatibility with the [IMS LTI Reference Implementation](https://github.com/IMSGlobal/lti-reference-implementation).

## Getting Started

There are two sample applications you can reference for ideas:
- [Sample Tool](https://github.com/andyfmiller/LtiAdvantageTool)
- [Sample Platform](https://github.com/andyfmiller/LtiAdvantagePlatform)