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
    /// Base for platform-to-tool launches that can advertise the platform's services to the tool:
    /// the resource link launches (through <see cref="LtiServiceLaunchRequest"/>) and
    /// <see cref="LtiAdvantage.DeepLinking.LtiDeepLinkingRequest"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The service claims live here rather than on <see cref="LtiRequest"/> on purpose: they are the
    /// platform telling the tool what it may call, so messages that travel tool to platform, such as
    /// <see cref="LtiStartAssessmentRequest"/>, must not inherit them.
    /// </para>
    /// <para>
    /// Deep linking is included because the AGS specification says the endpoint claim must be in
    /// any LTI message whose context gives the tool access to the services, and a tool often creates
    /// line items while handling a deep linking request. The proctoring messages are not included:
    /// the Proctoring Services specification lists the optional claims for those messages, and no
    /// service claim is among them.
    /// </para>
    /// </remarks>
    public abstract class LtiPlatformLaunchRequest : LtiRequest
    {
        /// <summary>Create an empty instance.</summary>
        protected LtiPlatformLaunchRequest()
        {
        }

        /// <summary>Create an instance with the claims.</summary>
        /// <param name="claims">A list of claims.</param>
        protected LtiPlatformLaunchRequest(IEnumerable<Claim> claims) : base(claims)
        {
        }

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
