using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LtiAdvantage.Lti;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc />
    /// <summary>
    /// LTI Deep Linking response.
    /// </summary>
    public class LtiDeepLinkingResponse : LtiRequest
    {
        /// <inheritdoc />
        /// <summary>
        /// Create an empty request.
        /// </summary>
        public LtiDeepLinkingResponse()
        {
            MessageType = Constants.Lti.LtiDeepLinkingResponseMessageType;
            Version = Constants.Lti.Version;
        }
        
        /// <inheritdoc />
        /// <summary>
        /// Create a request with the claims.
        /// </summary>
        /// <param name="claims">A list of claims.</param>
        public LtiDeepLinkingResponse(IEnumerable<Claim> claims) : base(claims)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a request with the claims in payload.
        /// </summary>
        /// <param name="payload"></param>
        public LtiDeepLinkingResponse(JwtPayload payload) : base(payload.Claims)
        {
        }

        /// <summary>
        /// A possibly empty list of content items.
        /// </summary>
        public ContentItem[] ContentItems
        {
            get { return this.GetClaimValue<ContentItem[]>(Constants.LtiClaims.ContentItems); }
            set { this.SetClaimValue(Constants.LtiClaims.ContentItems, value);}
        }

        /// <summary>
        /// The value from deep linking settings.
        /// </summary>
        public string Data
        {
            get { return this.GetClaimValue(Constants.LtiClaims.Data); }
            set { this.SetClaimValue(Constants.LtiClaims.Data, value);}
        }
        
        /// <summary>
        /// Optional plain text message.
        /// </summary>
        public string ErrorLog
        {
            get { return this.GetClaimValue(Constants.LtiClaims.ErrorLog); }
            set { this.SetClaimValue(Constants.LtiClaims.ErrorLog, value); }
        }

        /// <summary>
        /// Optional plain text message.
        /// </summary>
        public string ErrorMessage
        {
            get { return this.GetClaimValue(Constants.LtiClaims.ErrorMessage); }
            set { this.SetClaimValue(Constants.LtiClaims.ErrorMessage, value); }
        }

        /// <summary>
        /// Optional plain text message.
        /// </summary>
        public string Log
        {
            get { return this.GetClaimValue(Constants.LtiClaims.Log); }
            set { this.SetClaimValue(Constants.LtiClaims.Log, value); }
        }

        /// <summary>
        /// Optional plain text message.
        /// </summary>
        public string Message
        {
            get { return this.GetClaimValue(Constants.LtiClaims.Message); }
            set { this.SetClaimValue(Constants.LtiClaims.Message, value); }
        }
    }
}
