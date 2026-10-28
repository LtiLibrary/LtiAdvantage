# LtiAdvantage

LtiAdvantage is a library to ease the work of creating an LTI Advantage platform or tool. It has the
message, claim and service models, and supports

- [LTI Core Specification v1.3](https://www.imsglobal.org/spec/lti/v1p3/)
- LTI Advantage
  - [Assignment and Grade Services v2.0](https://www.imsglobal.org/spec/lti-ags/v2p0)
  - [Names and Role Provisioning Services v2.0](https://www.imsglobal.org/spec/lti-nrps/v2p0)
  - [Deep Linking v2.0](https://www.imsglobal.org/spec/lti-dl/v2p0)
- [Submission Review v1.0](https://www.imsglobal.org/spec/lti-sr/v1p0)
- [Dynamic Registration v1.0](https://www.imsglobal.org/spec/lti-dr/v1p0)
- [Course Groups Service v1.0](https://www.imsglobal.org/spec/lti-gs/v1p0)
- [Proctoring Services v1.0](https://www.imsglobal.org/spec/proctoring/v1p0)
- [IMS Security Framework v1.0](https://www.imsglobal.org/spec/security/v1p0/)

This library targets `netstandard2.0`. The other packages
([LtiAdvantage.AspNetCore](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.AspNetCore), [LtiAdvantage.IdentityModel](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.IdentityModel)
and [LtiAdvantage.Oidc](https://github.com/LtiLibrary/LtiAdvantage/tree/master/src/LtiAdvantage.Oidc)) target `net10.0`.

## Samples

Two sample applications show the libraries in use:
- [Sample Tool](https://github.com/LtiLibrary/LtiAdvantageTool)
- [Sample Platform](https://github.com/LtiLibrary/LtiAdvantagePlatform)
