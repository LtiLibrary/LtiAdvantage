using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    /// <inheritdoc />
    /// <summary>
    /// LTI End Assessment request, sent by the assessment platform to the proctoring tool when
    /// the candidate has finished, so the tool can close its proctoring session
    /// (https://www.imsglobal.org/spec/proctoring/v1p0#lti-end-assessment-message).
    /// </summary>
    /// <remarks>
    /// The platform only sends this message when the tool asked for it by setting
    /// <see cref="LtiStartAssessmentRequest.EndAssessmentReturn"/>.
    /// </remarks>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class LtiEndAssessmentRequest : LtiProctoringRequest
    {
        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiEndAssessmentRequest" /> with default
        /// values for the MessageType and Version claims.
        /// </summary>
        public LtiEndAssessmentRequest()
        {
            MessageType = Constants.Lti.LtiEndAssessmentRequestMessageType;
            Version = Constants.Lti.Version;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiEndAssessmentRequest" /> with the claims.
        /// </summary>
        /// <param name="claims">A list of claims.</param>
        public LtiEndAssessmentRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiEndAssessmentRequest" /> with the
        /// claims in payload.
        /// </summary>
        /// <param name="payload"></param>
        public LtiEndAssessmentRequest(JwtPayload payload) : base(payload.Claims)
        {
        }

        /// <summary>
        /// A message about the assessment intended for the end user.
        /// </summary>
        /// <remarks>
        /// This is the proctoring-specific errormsg claim, not the Deep Linking one.
        /// </remarks>
        public string ErrorMessage
        {
            get => this.GetClaimValue(Constants.LtiClaims.ProctoringErrorMessage);
            set => this.SetClaimValue(Constants.LtiClaims.ProctoringErrorMessage, value);
        }

        /// <summary>
        /// A message about the assessment intended for the tool's log.
        /// </summary>
        /// <remarks>
        /// This is the proctoring-specific errorlog claim, not the Deep Linking one.
        /// </remarks>
        public string ErrorLog
        {
            get => this.GetClaimValue(Constants.LtiClaims.ProctoringErrorLog);
            set => this.SetClaimValue(Constants.LtiClaims.ProctoringErrorLog, value);
        }
    }
}
