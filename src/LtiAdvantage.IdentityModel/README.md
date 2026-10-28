# LtiAdvantage.IdentityModel

LtiAdvantage.IdentityModel has the tool-side pieces of LTI Advantage:

- an HttpClient extension method to request a token using JWT client credentials in the
  [IMS recommended format](https://www.imsglobal.org/spec/security/v1p0#using-json-web-tokens-with-oauth-2-0-client-credentials-grant)
  for client-credentials grant;
- `LtiTokenValidator`, which validates a launch's ID token, with `InMemoryNonceStore` for replay detection;
- HttpClient extensions for calling Assignment and Grade Services, Names and Role Provisioning
  Services and Course Groups (`HttpClientLtiServiceExtensions`), which follow paging and report
  ProblemDetails errors;
- the tool side of Dynamic Registration.

This library targets `net10.0` and uses [IdentityModel](https://github.com/IdentityModel/IdentityModel).

## Getting Started


```
var tokenResponse = await httpClient.RequestClientCredentialsTokenWithJwtAsync(
    new JwtClientCredentialsTokenRequest
    {
        Address = tokenEndPoint,
        Jwt = jwt,
        Scope = Constants.LtiScopes.NamesRoleReadonly
     });
```

To sign the JWT, read the tool's PEM private key with `RsaPemKeys`, passing the key id it is
published under in the tool's JWKS so the platform can pick the right key after a rotation:

```
var credentials = RsaPemKeys.SigningCredentialsFromPem(privateKeyPem, keyId);
```

`AccessTokenUtil.GetAccessTokenAsync(httpClient, issuer, scopes, clientId, tokenUrl, privateKeyPem, keyId)`
does both steps. `RsaPemKeys` uses .NET's own PEM support; it reads PKCS#1, PKCS#8 and
SubjectPublicKeyInfo keys.

## Samples

Two sample applications show the libraries in use:
- [Sample Tool](https://github.com/LtiLibrary/LtiAdvantageTool)
- [Sample Platform](https://github.com/LtiLibrary/LtiAdvantagePlatform)
