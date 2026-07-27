using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.NamesRoleProvisioningService;
using LtiAdvantage.SubmissionReview;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    /// <inheritdoc />
    /// <summary>
    /// LTI Submission Review request (https://www.imsglobal.org/spec/lti-sr/v1p0).
    /// </summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class LtiSubmissionReviewRequest : LtiRequest
    {
        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiSubmissionReviewRequest" /> with default
        /// values for the MessageType and Version claims.
        /// </summary>
        public LtiSubmissionReviewRequest()
        {
            MessageType = Constants.Lti.LtiSubmissionReviewRequestMessageType;
            Version = Constants.Lti.Version;
        }

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiSubmissionReviewRequest" /> with the claims.
        /// </summary>
        /// <param name="claims">A list of claims.</param>
        public LtiSubmissionReviewRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create an instance of <see cref="T:LtiAdvantage.Lti.LtiSubmissionReviewRequest" /> with the
        /// claims in payload.
        /// </summary>
        /// <param name="payload"></param>
        public LtiSubmissionReviewRequest(JwtPayload payload) : base(payload.Claims)
        {
        }

        /// <summary>The Assignment and Grade Services claim (the lineitem under review).</summary>
        public AssignmentGradeServicesClaimValueType AssignmentGradeServices
        {
            get => this.GetClaimValue<AssignmentGradeServicesClaimValueType>(Constants.LtiClaims.AssignmentGradeServices);
            set => this.SetClaimValue(Constants.LtiClaims.AssignmentGradeServices, value);
        }

        /// <summary>The Names and Roles Provisioning Service claim.</summary>
        public NamesRoleServiceClaimValueType NamesRoleService
        {
            get => this.GetClaimValue<NamesRoleServiceClaimValueType>(Constants.LtiClaims.NamesRoleService);
            set => this.SetClaimValue(Constants.LtiClaims.NamesRoleService, value);
        }

        /// <summary>The resource_link claim (same shape as in LtiResourceLinkRequest).</summary>
        public ResourceLinkClaimValueType ResourceLink
        {
            get => this.GetClaimValue<ResourceLinkClaimValueType>(Constants.LtiClaims.ResourceLink);
            set => this.SetClaimValue(Constants.LtiClaims.ResourceLink, value);
        }

        /// <summary>The for_user claim (the user whose submission is being reviewed).</summary>
        public ForUserClaimValueType ForUser
        {
            get => this.GetClaimValue<ForUserClaimValueType>(Constants.LtiClaims.ForUser);
            set => this.SetClaimValue(Constants.LtiClaims.ForUser, value);
        }
    }
}
