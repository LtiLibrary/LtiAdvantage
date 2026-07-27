using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LtiAdvantage.Proctoring;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    /// <inheritdoc />
    /// <summary>
    /// LTI Start Assessment request, sent by the proctoring tool back to the assessment
    /// platform once the candidate has been verified and the assessment may begin
    /// (https://www.imsglobal.org/spec/proctoring/v1p0#lti-start-assessment-message).
    /// </summary>
    /// <remarks>
    /// This is the one message in the proctoring flow that travels tool to platform. It is
    /// POSTed to the <see cref="LtiStartProctoringRequest.StartAssessmentUrl"/> the platform
    /// supplied, and it must carry back the platform's
    /// <see cref="LtiStartProctoringRequest.SessionData"/> unchanged.
    /// </remarks>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class LtiStartAssessmentRequest : LtiRequest
    {
        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiStartAssessmentRequest" /> with default
        /// values for the MessageType and Version claims.
        /// </summary>
        public LtiStartAssessmentRequest()
        {
            MessageType = Constants.Lti.LtiStartAssessmentRequestMessageType;
            Version = Constants.Lti.Version;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiStartAssessmentRequest" /> with the claims.
        /// </summary>
        /// <param name="claims">A list of claims.</param>
        public LtiStartAssessmentRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiStartAssessmentRequest" /> with the
        /// claims in payload.
        /// </summary>
        /// <param name="payload"></param>
        public LtiStartAssessmentRequest(JwtPayload payload) : base(payload.Claims)
        {
        }

        /// <summary>The resource_link claim identifying the assessment. Required.</summary>
        public ResourceLinkClaimValueType ResourceLink
        {
            get => this.GetClaimValue<ResourceLinkClaimValueType>(Constants.LtiClaims.ResourceLink);
            set => this.SetClaimValue(Constants.LtiClaims.ResourceLink, value);
        }

        /// <summary>
        /// The attempt this proctoring session covers, starting at 1. Required.
        /// </summary>
        public int? AttemptNumber
        {
            get => this.GetClaimValue<int?>(Constants.LtiClaims.AttemptNumber);
            set => this.SetClaimValue(Constants.LtiClaims.AttemptNumber, value);
        }

        /// <summary>
        /// The opaque value the platform issued on the
        /// <see cref="LtiStartProctoringRequest"/>, echoed back unchanged. Required.
        /// </summary>
        public string SessionData
        {
            get => this.GetClaimValue(Constants.LtiClaims.SessionData);
            set => this.SetClaimValue(Constants.LtiClaims.SessionData, value);
        }

        /// <summary>
        /// Set by the tool to ask the platform to send an <see cref="LtiEndAssessmentRequest"/>
        /// when the candidate finishes, so the tool can close its proctoring session.
        /// </summary>
        public bool? EndAssessmentReturn
        {
            get => this.GetClaimValue<bool?>(Constants.LtiClaims.EndAssessmentReturn);
            set => this.SetClaimValue(Constants.LtiClaims.EndAssessmentReturn, value);
        }

        /// <summary>
        /// The user identity attributes the tool verified at check-in.
        /// </summary>
        public VerifiedUserClaimValueType VerifiedUser
        {
            get => this.GetClaimValue<VerifiedUserClaimValueType>(Constants.LtiClaims.VerifiedUser);
            set => this.SetClaimValue(Constants.LtiClaims.VerifiedUser, value);
        }

        /// <summary>
        /// Opaque proctoring configuration the tool asks the platform to store for later
        /// launches of this attempt.
        /// </summary>
        public ProctoringSettingsClaimValueType ProctoringSettings
        {
            get => this.GetClaimValue<ProctoringSettingsClaimValueType>(Constants.LtiClaims.ProctoringSettings);
            set => this.SetClaimValue(Constants.LtiClaims.ProctoringSettings, value);
        }
    }
}
