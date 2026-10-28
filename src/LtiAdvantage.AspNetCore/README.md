# LtiAdvantage.AspNetCore

This library provides ASP.NET Core controller base classes for the platform side of the LTI
Advantage services (AGS, NRPS, Course Groups, Proctoring), Dynamic Registration, the OpenID
configuration and JWKS publishing. It targets `net10.0`.

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
`Jwks.JwksService`. A service route's `.{format}` alias moves with it, and `Url.Link(routeName, …)`
follows the new path. Keep the route's parameters (such as `{contextId}`) in the new template. A
name that matches no route stops the app at startup.

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
