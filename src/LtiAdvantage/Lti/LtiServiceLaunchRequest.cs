using System.Collections.Generic;
using System.Security.Claims;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    /// <inheritdoc />
    /// <summary>
    /// Base for platform-to-tool launches that happen in the context of a resource link:
    /// <see cref="LtiResourceLinkRequest"/> and <see cref="LtiSubmissionReviewRequest"/>.
    /// Adds the resource link claim to the service claims declared on
    /// <see cref="LtiPlatformLaunchRequest"/>.
    /// </summary>
    public abstract class LtiServiceLaunchRequest : LtiPlatformLaunchRequest
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
    }
}
