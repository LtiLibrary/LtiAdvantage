﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using LtiAdvantageLibrary.NamesRoleService;
using LtiAdvantageLibrary.Utilities;

namespace LtiAdvantageLibrary.Lti
{
    /// <summary>
    /// https://purl.imsglobal.org/spec/lti/v1p3/schema/json/LtiResourceLinkRequest.json
    /// </summary>
    public class LtiResourceLinkRequest : JwtPayload
    {

        #region Constructors

        public LtiResourceLinkRequest()
        {
            MessageType = Constants.Lti.LtiResourceLinkRequestMessageType;
            Version = Constants.Lti.Version;
        }

        public LtiResourceLinkRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        public LtiResourceLinkRequest(JwtPayload payload) : base(payload.Claims)
        {
        }

        #endregion

        #region Required Message Claims

        // See https://www.imsglobal.org/spec/lti/v1p3/#required-message-claims
        // See https://openid.net/specs/openid-connect-core-1_0.html#Claims
        // See https://purl.imsglobal.org/spec/lti/v1p3/schema/json/Token.json

        /// <summary>
        /// REQUIRED. Audience(s) for whom this ID Token is intended i.e. the Tool.
        /// It MUST contain the OAuth 2.0 client_id of the Tool as an audience value.
        /// It MAY also contain identifiers for other audiences. In the general case,
        /// the aud value is an array of case-sensitive strings. In the common special
        /// case when there is one audience, the aud value MAY be a single case-sensitive string.
        /// </summary>
        public string[] Audiences
        {
            get { return this.GetClaimValue<string[]>(JwtRegisteredClaimNames.Aud); }
            set { this.SetClaimValue(JwtRegisteredClaimNames.Aud, value);}
        }

        /// <summary>
        /// The required https://purl.imsglobal.org/spec/lti/claim/deployment_id claim's value
        /// contains a string that identifies the platform-tool integration governing the message.
        /// </summary>
        public string DeploymentId
        {
            get { return this.GetClaimValue(Constants.LtiClaims.DeploymentId); }
            set { this.SetClaimValue(Constants.LtiClaims.DeploymentId, value); }
        }

        /// <summary>
        /// The type of LTI message. Must be "LtiResourceLinkRequest".
        /// </summary>
        public string MessageType
        {
            get { return this.GetClaimValue(Constants.LtiClaims.MessageType); }
            set { this.SetClaimValue(Constants.LtiClaims.MessageType, value); }
        }

        /// <summary>
        /// The Names and Roles Provisioning Service claim.
        /// </summary>
        public NamesRoleServiceClaimValueType NamesRoleService
        {
            get { return this.GetClaimValue<NamesRoleServiceClaimValueType>(Constants.LtiClaims.NamesRoleService); }
            set { this.SetClaimValue(Constants.LtiClaims.NamesRoleService, value);}
        }

        /// <summary>
        /// Value used to associate a Client session with an ID Token.
        /// Should only be used once. Use GenerateCryptoNonce to generate
        /// a cryptographic nonce value. Required.
        /// <example>
        /// LtiResourceLinkRequest request;
        /// request.Nonce = LtiResourceLinkRequest.GenerateCryptographicNonce();
        /// </example>
        /// </summary>
        public new string Nonce
        {
            get { return base.Nonce; }
            set { this.SetClaimValue(JwtRegisteredClaimNames.Nonce, value); }
        }

        /// <summary>
        /// The required https://purl.imsglobal.org/spec/lti/claim/resource_link claim composes
        /// properties for the resource link from which the launch message occurs.
        /// <example>
        /// {
        ///   "id": "200d101f-2c14-434a-a0f3-57c2a42369fd",
        ///   ...
        /// }
        /// </example>
        /// </summary>
        public ResourceLinkClaimValueType ResourceLink
        {
            get { return this.GetClaimValue<ResourceLinkClaimValueType>(Constants.LtiClaims.ResourceLink); }
            set { this.SetClaimValue(Constants.LtiClaims.ResourceLink, value); }
        }

        /// <summary>
        /// An array of roles as defined in the Core LTI specification.
        /// </summary>
        public Role[] Roles
        {
            get { return this.GetClaimValue<Role[]>(Constants.LtiClaims.Roles); }
            set { this.SetClaimValue(Constants.LtiClaims.Roles, value); }
        }

        /// <summary>
        /// The required 'sub' claim's value contains a string acting as an opaque identifier for
        /// the user that initiated the launch. This value MUST be immutable and MUST be unique
        /// within the platform instance.
        /// </summary>
        public string UserId
        {
            get { return this.GetClaimValue(JwtRegisteredClaimNames.Sub); }
            set { this.SetClaimValue(JwtRegisteredClaimNames.Sub, value); }
        }

        /// <summary>
        /// The version to which the message conforms. Must be "1.3.0".
        /// </summary>
        public string Version
        {
            get { return this.GetClaimValue(Constants.LtiClaims.Version); }
            set { this.SetClaimValue(Constants.LtiClaims.Version, value); }
        }

        #endregion

        #region Optional Message Claims

        /// <summary>
        /// Properties of the context from which the launch originated (for example, course id and title).
        /// </summary>
        public ContextClaimValueType Context
        {
            get { return this.GetClaimValue<ContextClaimValueType>(Constants.LtiClaims.Context); }
            set { this.SetClaimValue(Constants.LtiClaims.Context, value); }
        }

        /// <summary>
        /// This is a map of key/value custom parameters which are to be included with the launch.
        /// </summary>
        public Dictionary<string, string> Custom
        {
            get { return this.GetClaimValue<Dictionary<string, string>>(Constants.LtiClaims.Custom); }
            set { this.SetClaimValue(Constants.LtiClaims.Custom, value);}
        }

        /// <summary>
        /// Information to help the Tool present itself appropriately.
        /// </summary>
        public LaunchPresentationClaimValueType LaunchPresentation
        {
            get { return this.GetClaimValue<LaunchPresentationClaimValueType>(Constants.LtiClaims.LaunchPresentation); }
            set { this.SetClaimValue(Constants.LtiClaims.LaunchPresentation, value); }
        }

        /// <summary>
        /// Properties about available Learning Information Services (LIS),
        /// usually originating from the Student Information System.
        /// </summary>
        public LisClaimValueType Lis
        {
            get { return this.GetClaimValue<LisClaimValueType>(Constants.LtiClaims.Lis); }
            set { this.SetClaimValue(Constants.LtiClaims.Lis, value);}
        }

        /// <summary>
        /// Properties associated with the platform initiating the launch.
        /// </summary>
        public PlatformClaimValueType Platform
        {
            get { return this.GetClaimValue<PlatformClaimValueType>(Constants.LtiClaims.Platform); }
            set { this.SetClaimValue(Constants.LtiClaims.Platform, value); }
        }

        /// <summary>
        /// An array of the user_id ('sub' claim) values which the current user can access as a mentor.
        /// </summary>
        public string[] RoleScopeMentor
        {
            get { return this.GetClaimValue<string[]>(Constants.LtiClaims.RoleScopeMentor); }
            set { this.SetClaimValue(Constants.LtiClaims.RoleScopeMentor, value); }
        }

        #endregion

        #region Optional OpenID Connect claims
        
        // See https://www.iana.org/assignments/jwt/jwt.xhtml#claims
        // See https://openid.net/specs/openid-connect-core-1_0.html#Claims
        // See https://purl.imsglobal.org/spec/lti/v1p3/schema/json/Token.json

        /// <summary>
        /// End-User's preferred e-mail address. Its value MUST conform to the RFC 5322 [RFC5322]
        /// addr-spec syntax. The Tool MUST NOT rely upon this value being unique.
        /// <example>
        /// "jane@example.org"
        /// </example>
        /// </summary>
        public string Email
        {
            get { return this.GetClaimValue(JwtRegisteredClaimNames.Email); }
            set { this.SetClaimValue(JwtRegisteredClaimNames.Email, value); }
        }

        /// <summary>
        /// Surname(s) or last name(s) of the End-User. Note that in some cultures, people can have
        /// multiple family names or no family name; all can be present, with the names being separated
        /// by space characters.
        /// <example>
        /// "Doe"
        /// </example>
        /// </summary>
        public string FamilyName
        {
            get { return this.GetClaimValue(JwtRegisteredClaimNames.FamilyName); }
            set { this.SetClaimValue(JwtRegisteredClaimNames.FamilyName, value);}
        }

        /// <summary>
        /// Given name(s) or first name(s) of the End-User. Note that in some cultures, people can have
        /// multiple given names; all can be present, with the names being separated by space characters.
        /// <example>
        /// "Jane"
        /// </example>
        /// </summary>
        public string GivenName
        {
            get { return this.GetClaimValue(JwtRegisteredClaimNames.GivenName); }
            set { this.SetClaimValue(JwtRegisteredClaimNames.GivenName, value);}
        }

        /// <summary>
        /// End-User's locale, represented as a BCP47 [RFC5646] language tag. This is typically an
        /// ISO 639-1 Alpha-2 [ISO639‑1] language code in lowercase and an ISO 3166-1 Alpha-2 [ISO3166‑1]
        /// country code in uppercase, separated by a dash. For example, en-US or fr-CA. As a compatibility
        /// note, some implementations have used an underscore as the separator rather than a dash, for
        /// example, en_US; Tools MAY choose to accept this locale syntax as well.
        /// <example>
        /// en-US
        /// </example>
        /// </summary>
        public string Locale
        {
            get { return this.GetClaimValue(Constants.OidcClaims.Locale); }
            set { this.SetClaimValue(Constants.OidcClaims.Locale, value); }
        }

        /// <summary>
        /// Middle name(s) of the End-User. Note that in some cultures, people can have multiple middle
        /// names; all can be present, with the names being separated by space characters. Also note that
        /// in some cultures, middle names are not used.
        /// <example>
        /// "Marie"
        /// </example>
        /// </summary>
        public string MiddleName
        {
            get { return this.GetClaimValue(Constants.OidcClaims.MiddleName); }
            set { this.SetClaimValue(Constants.OidcClaims.MiddleName, value);}
        }

        /// <summary>
        /// End-User's full name in displayable form including all name parts, possibly including titles and
        /// suffixes, ordered according to the End-User's locale and preferences.
        /// <example>
        /// "Ms. Jane Marie Doe"
        /// </example>
        /// </summary>
        public string Name
        {
            get { return this.GetClaimValue(Constants.OidcClaims.Name); }
            set { this.SetClaimValue(Constants.OidcClaims.Name, value);}
        }

        /// <summary>
        /// URL of the End-User's profile picture. This URL MUST refer to an image file (for example, a PNG,
        /// JPEG, or GIF image file), rather than to a Web page containing an image. Note that this URL SHOULD
        /// specifically reference a profile photo of the End-User suitable for displaying when describing the
        /// End-User, rather than an arbitrary photo taken by the End-User.
        /// <example>
        /// "http://example.org/jane.jpg"
        /// </example>
        /// </summary>
        public string Picture
        {
            get { return this.GetClaimValue(Constants.OidcClaims.Picture); }
            set { this.SetClaimValue(Constants.OidcClaims.Picture, value);}
        }

        #endregion

        /// <summary>
        /// Generate a cryptographic nonce.
        /// </summary>
        /// <returns>
        /// A cryptographic nonce as a string value.
        /// </returns>
        public static string GenerateCryptographicNonce()
        {
            using (var provider = new RNGCryptoServiceProvider())
            {
                var byteArray = new byte[4];
                provider.GetBytes(byteArray);
                var randomInteger = BitConverter.ToUInt32(byteArray, 0);
                return randomInteger.ToString();
            }
       }
    }
}
