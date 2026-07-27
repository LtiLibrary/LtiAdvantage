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
    /// LTI Start Proctoring request, sent by the assessment platform to the proctoring tool
    /// to open a proctoring session before the assessment begins
    /// (https://www.imsglobal.org/spec/proctoring/v1p0#lti-start-proctoring-message).
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class LtiStartProctoringRequest : LtiRequest
    {
        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiStartProctoringRequest" /> with default
        /// values for the MessageType and Version claims.
        /// </summary>
        public LtiStartProctoringRequest()
        {
            MessageType = Constants.Lti.LtiStartProctoringRequestMessageType;
            Version = Constants.Lti.Version;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiStartProctoringRequest" /> with the claims.
        /// </summary>
        /// <param name="claims">A list of claims.</param>
        public LtiStartProctoringRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiStartProctoringRequest" /> with the
        /// claims in payload.
        /// </summary>
        /// <param name="payload"></param>
        public LtiStartProctoringRequest(JwtPayload payload) : base(payload.Claims)
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
        /// The platform URL the proctoring tool submits the
        /// <see cref="LtiStartAssessmentRequest"/> to once the candidate is verified. Required.
        /// </summary>
        public string StartAssessmentUrl
        {
            get => this.GetClaimValue(Constants.LtiClaims.StartAssessmentUrl);
            set => this.SetClaimValue(Constants.LtiClaims.StartAssessmentUrl, value);
        }

        /// <summary>
        /// Opaque value the tool must echo back on the <see cref="LtiStartAssessmentRequest"/>
        /// so the platform can correlate the two messages. Required.
        /// </summary>
        public string SessionData
        {
            get => this.GetClaimValue(Constants.LtiClaims.SessionData);
            set => this.SetClaimValue(Constants.LtiClaims.SessionData, value);
        }

        /// <summary>
        /// The Assessment Control Service claim, present when the platform lets the proctoring
        /// tool pause, resume, terminate, update or flag the attempt.
        /// </summary>
        public AssessmentControlClaimValueType Acs
        {
            get => this.GetClaimValue<AssessmentControlClaimValueType>(Constants.LtiClaims.Acs);
            set => this.SetClaimValue(Constants.LtiClaims.Acs, value);
        }

        /// <summary>
        /// Opaque proctoring configuration the platform holds on the tool's behalf.
        /// </summary>
        public ProctoringSettingsClaimValueType ProctoringSettings
        {
            get => this.GetClaimValue<ProctoringSettingsClaimValueType>(Constants.LtiClaims.ProctoringSettings);
            set => this.SetClaimValue(Constants.LtiClaims.ProctoringSettings, value);
        }
    }
}
