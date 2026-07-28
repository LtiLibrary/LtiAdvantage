using System.Collections.Generic;
using System.Security.Claims;
using LtiAdvantage.AssignmentGradeServices;
using LtiAdvantage.CourseGroupsService;
using LtiAdvantage.NamesRoleProvisioningService;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    /// <inheritdoc />
    /// <summary>
    /// Base for platform-to-tool launches that happen in the context of a resource link and can
    /// advertise the platform's services to the tool: <see cref="LtiResourceLinkRequest"/> and
    /// <see cref="LtiSubmissionReviewRequest"/>.
    /// </summary>
    /// <remarks>
    /// The service claims live here rather than on <see cref="LtiRequest"/> on purpose: they are the
    /// platform telling the tool what it may call, so messages that do not travel platform to tool,
    /// such as the deep linking response, must not inherit them.
    /// </remarks>
    public abstract class LtiServiceLaunchRequest : LtiRequest
    {
        /// <summary>Create an empty instance.</summary>
        protected LtiServiceLaunchRequest()
        {
        }

        /// <summary>Create an instance with the claims.</summary>
        /// <param name="claims">A list of claims.</param>
        protected LtiServiceLaunchRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

        #region Resource Link Claim

        // See https://www.imsglobal.org/spec/lti/v1p3/#resource-link-claim

        /// <summary>
        /// The https://purl.imsglobal.org/spec/lti/claim/resource_link claim composes
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

        #endregion

        #region Optional Service Claims

        // Each of these is the platform advertising a service the tool may call against this
        // launch's context. All are optional: a platform that does not offer the service, or
        // does not grant this deployment access to it, omits the claim.

        /// <summary>
        /// The Assignment and Grade Services claim.
        /// </summary>
        public AssignmentGradeServicesClaimValueType AssignmentGradeServices
        {
            get { return this.GetClaimValue<AssignmentGradeServicesClaimValueType>(Constants.LtiClaims.AssignmentGradeServices); }
            set { this.SetClaimValue(Constants.LtiClaims.AssignmentGradeServices, value); }
        }

        /// <summary>
        /// The Course Groups Service claim, present when the platform exposes the groups
        /// (and optionally the group sets) endpoint for this context.
        /// </summary>
        public CourseGroupsServiceClaimValueType CourseGroupsService
        {
            get { return this.GetClaimValue<CourseGroupsServiceClaimValueType>(Constants.LtiClaims.CourseGroupsService); }
            set { this.SetClaimValue(Constants.LtiClaims.CourseGroupsService, value); }
        }

        /// <summary>
        /// The Names and Roles Provisioning Service claim.
        /// </summary>
        public NamesRoleServiceClaimValueType NamesRoleService
        {
            get { return this.GetClaimValue<NamesRoleServiceClaimValueType>(Constants.LtiClaims.NamesRoleService); }
            set { this.SetClaimValue(Constants.LtiClaims.NamesRoleService, value); }
        }

        #endregion
    }
}
