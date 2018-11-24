# LtiAdvantage.IdentityServer4

LtiAdvantage.IdentityServer4 has an Identity Server 4 secret validator that understands the 
[IMS recommended format](https://www.imsglobal.org/spec/security/v1p0#using-json-web-tokens-with-oauth-2-0-client-credentials-grant)
of client-credentials grant. This is only useful if you use IdentityServer4 to serve up authorization tokens.

## .NET Standard

This library targets `netstandard2.0` and uses [BouncyCastle.NetCore](https://github.com/chrishaly/bc-csharp) to read PEM formatted keys
for compatibility with the [IMS LTI Reference Implementation](https://github.com/IMSGlobal/lti-reference-implementation).

## Getting Started

Add the `PublicKeyJwtSecretValidator` to Startup.cs on the identity server (may be your platform):
```
// Add JWT client credentials validation
.AddSecretParser<JwtBearerClientAssertionSecretParser>()
.AddSecretValidator<PublicKeyJwtSecretValidator>()
```

There are two sample applications you can reference for ideas:
- [Sample Tool](https://github.com/andyfmiller/LtiAdvantageTool)
- [Sample Platform](https://github.com/andyfmiller/LtiAdvantagePlatform)