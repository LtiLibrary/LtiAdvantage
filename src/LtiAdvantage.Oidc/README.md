# LtiAdvantage.IdentityServer4

This project has two Identity Server 4 extensions:

1. A secret validator that understands the 
[IMS recommended format](https://www.imsglobal.org/spec/security/v1p0#using-json-web-tokens-with-oauth-2-0-client-credentials-grant)
of client-credentials grant.
2. Impersonation support that allows a user (such as an admin) impersonate another user (such as a member of a course).

## .NET Standard

This library targets `netstandard2.0`.

## Getting Started

Add the secret validator to IdentityServer in `ConfigureServices`:
```
services.AddIdentityServer()
  .AddLtiJwtBearerClientAuthentication();
```

Add impersonation support to IdentityServer in `ConfigureServices`:
```
services.AddIdentityServer()
  .AddImpersonationSupport();
```

There are two sample applications you can reference for ideas:
- [Sample Tool](https://github.com/andyfmiller/LtiAdvantageTool)
- [Sample Platform](https://github.com/andyfmiller/LtiAdvantagePlatform)
