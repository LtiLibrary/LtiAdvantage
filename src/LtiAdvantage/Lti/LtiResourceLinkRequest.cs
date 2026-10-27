using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LtiAdvantage.Lti
{
    /// <inheritdoc />
    /// <summary>
    /// https://purl.imsglobal.org/spec/lti/v1p3/schema/json/LtiResourceLinkRequest.json
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class LtiResourceLinkRequest : LtiServiceLaunchRequest
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiResourceLinkRequest" /> with default values
        /// for the MessageType and Version claims.
        /// </summary>
        public LtiResourceLinkRequest()
        {
            MessageType = Constants.Lti.LtiResourceLinkRequestMessageType;
            Version = Constants.Lti.Version;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiResourceLinkRequest" /> with the claims.
        /// </summary>
        /// <param name="claims">A list of claims.</param>
        public LtiResourceLinkRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiResourceLinkRequest" /> with the
        /// claims in payload.
        /// </summary>
        /// <param name="payload"></param>
        public LtiResourceLinkRequest(JwtPayload payload) : base(payload.Claims)
        {
        }

        #endregion
    }
}
