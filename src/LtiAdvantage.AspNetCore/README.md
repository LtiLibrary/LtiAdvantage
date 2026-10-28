# LtiAdvantage.AspNetCore

This library provides ASP.NET Core controller implementations to support platform development.
It targets `net10.0`
It uses BouncyCastle.NetCore to read and write PEM formatted keys
for compatibility with the [IMS LTI Reference Implementation](https://github.com/IMSGlobal/lti-reference-implementation).

## Running next to an OpenID Connect server

`OpenIdConfigurationControllerBase` serves the platform's discovery document at
`.well-known/openid-configuration`. An OpenID Connect server in the same app, such as OpenIddict,
serves its own document at that path, and the two routes clash. Do one of the following:

- Turn off the server's discovery endpoint. With OpenIddict:
  `options.SetConfigurationEndpointUris(Array.Empty<string>())`.
- Or move the LTI document. Tools find it through the `openid_configuration` URL the platform
  sends when it starts a registration, so any path works:

  ```csharp
  services.AddControllers(options => options.MapLtiRoute(
      Constants.ServiceEndpoints.OpenIdConnect.OpenIdConfigurationService,
      "lti/.well-known/openid-configuration"));
  ```

`MapLtiRoute` works for any of the named routes in `Constants.ServiceEndpoints`, for example
`Jwks.JwksService`. `Url.Link(routeName, …)` follows the new path. A name that matches no route
stops the app at startup.
