# LtiAdvantage.Oidc

This library extends an [OpenIddict](https://documentation.openiddict.com/) server for an LTI
platform. It replaces LtiAdvantage.IdentityServer4.

It adds impersonation support, which lets a user (such as an admin) act as another user (such as a
member of a course) during the authorize request.

This library targets `net10.0`.

## Getting Started

Add impersonation support to the OpenIddict server in `ConfigureServices`:
```
services.AddOpenIddict()
  .AddServer(options => options.AddImpersonationSupport());
```

## Samples

Two sample applications show the libraries in use:
- [Sample Tool](https://github.com/LtiLibrary/LtiAdvantageTool)
- [Sample Platform](https://github.com/LtiLibrary/LtiAdvantagePlatform)
