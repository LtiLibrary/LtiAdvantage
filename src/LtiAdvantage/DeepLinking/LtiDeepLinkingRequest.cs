using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LtiAdvantage.Lti;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc />
    /// <summary>
    /// LTI Deep Linking request.
    /// </summary>
    public class LtiDeepLinkingRequest : LtiRequest
    {
        /// <inheritdoc />
        /// <summary>
        /// Create an empty request.
        /// </summary>
        public LtiDeepLinkingRequest()
        {
            MessageType = Constants.Lti.LtiDeepLinkingRequestMessageType;
            Version = Constants.Lti.Version;
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Create a request with the claims.
        /// </summary>
        /// <param name="claims">A list of claims.</param>
        public LtiDeepLinkingRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a request with the claims in payload.
        /// </summary>
        /// <param name="payload"></param>
        public LtiDeepLinkingRequest(JwtPayload payload) : base(payload.Claims)
        {
        }

        /// <summary>
        /// Deep Linking settings.
        /// </summary>
        public DeepLinkingSettingsClaimValueType DeepLinkingSettings
        {
            get { return this.GetClaimValue<DeepLinkingSettingsClaimValueType>(Constants.LtiClaims.DeepLinkingSettings); }
            set { this.SetClaimValue(Constants.LtiClaims.DeepLinkingSettings, value);}
        }
    }
}
