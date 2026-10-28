using System.Collections.Generic;
using System.Security.Claims;
using LtiAdvantage.Proctoring;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    /// <inheritdoc />
    /// <summary>
    /// Base for the three Proctoring Services messages: <see cref="LtiStartProctoringRequest"/>,
    /// <see cref="LtiStartAssessmentRequest"/> and <see cref="LtiEndAssessmentRequest"/>.
    /// Declares the claims they share about the attempt being proctored.
    /// (https://www.imsglobal.org/spec/proctoring/v1p0#messages)
    /// </summary>
    /// <remarks>
    /// This base does not derive from <see cref="LtiPlatformLaunchRequest"/>, so the proctoring
    /// messages carry no service claims. The specification lists the optional claims for each
    /// message and none of them is a service claim, and the Start Assessment message travels
    /// tool to platform, so it could not advertise a platform service anyway.
    /// </remarks>
    public abstract class LtiProctoringRequest : LtiRequest
    {
        /// <summary>Create an empty instance.</summary>
        protected LtiProctoringRequest()
        {
        }

        /// <summary>Create an instance with the claims.</summary>
        /// <param name="claims">A list of claims.</param>
        protected LtiProctoringRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        /// <summary>
        /// The resource_link claim identifying the assessment. Required on the Start Proctoring
        /// and Start Assessment messages.
        /// </summary>
        /// <remarks>
        /// For End Assessment, section 4.4 of the specification lists resource_link as neither
        /// required nor optional, but the End Assessment example in section 6.1 includes it. Set it
        /// when the tool needs to know which assessment ended.
        /// </remarks>
        public ResourceLinkClaimValueType ResourceLink
        {
            get => this.GetClaimValue<ResourceLinkClaimValueType>(Constants.LtiClaims.ResourceLink);
            set => this.SetClaimValue(Constants.LtiClaims.ResourceLink, value);
        }

        /// <summary>
        /// The candidate's attempt number, starting at 1. Required on every proctoring message.
        /// </summary>
        /// <remarks>
        /// On Start Proctoring and Start Assessment it is the attempt the proctoring session
        /// covers. On End Assessment it is the attempt that just ended, which lets the tool match
        /// the message to its open proctoring session.
        /// </remarks>
        public int? AttemptNumber
        {
            get => this.GetClaimValue<int?>(Constants.LtiClaims.AttemptNumber);
            set => this.SetClaimValue(Constants.LtiClaims.AttemptNumber, value);
        }

        /// <summary>
        /// Opaque proctoring configuration the platform stores on the tool's behalf. Optional.
        /// </summary>
        /// <remarks>
        /// The direction depends on the message. On Start Proctoring and End Assessment the
        /// platform sends the settings it holds to the tool. On Start Assessment the tool sets
        /// them to ask the platform to store them for later launches of this attempt.
        /// </remarks>
        public ProctoringSettingsClaimValueType ProctoringSettings
        {
            get => this.GetClaimValue<ProctoringSettingsClaimValueType>(Constants.LtiClaims.ProctoringSettings);
            set => this.SetClaimValue(Constants.LtiClaims.ProctoringSettings, value);
        }
    }
}
