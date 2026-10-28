using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LtiAdvantage.Lti;
using LtiAdvantage.NamesRoleProvisioningService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using LtiAdvantage.AspNetCore.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

namespace LtiAdvantage.AspNetCore.NamesRoleProvisioningService
{
    /// <inheritdoc cref="ControllerBase" />
    /// <summary>
    /// Implements the Names and Role Provisioning Service membership endpoint.
    /// </summary>
    [ApiController]
    [ServiceExceptionFilter]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public abstract class MembershipControllerBase : ControllerBase, IMembershipController
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MembershipControllerBase> _logger;

        /// <summary>
        /// </summary>
        protected MembershipControllerBase(IWebHostEnvironment env, ILogger<MembershipControllerBase> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// Returns the membership.
        /// </summary>
        protected abstract Task<ActionResult<MembershipContainer>> OnGetMembershipAsync(GetMembershipRequest request);

        /// <summary>
        /// Returns the membership of a context.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="limit">Optional limit to the number of members to return.</param>
        /// <param name="rlid">Optional resource link filter for members with access to resource link.</param>
        /// <param name="role">Optional role filter for members that have the specified role.</param>
        /// <param name="groups">
        /// Optional Course Groups Service 1.0 request for each member's group enrollments.
        /// Only honoured when the caller also holds the Course Groups scope
        /// (<see cref="Constants.LtiScopes.Gs.ContextGroupReadonly"/>); otherwise the request is
        /// served without groups, so a tool granted only NRPS cannot read group data.
        /// See https://www.imsglobal.org/spec/lti-gs/v1p0#membership-service-with-groups.
        /// </param>
        /// <returns>The members.</returns>
        [HttpGet]
        [Produces(Constants.MediaTypes.MembershipContainer)]
        [ProducesResponseType(typeof(MembershipContainer), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, 
            Policy = Constants.LtiScopes.Nrps.MembershipReadonly)]
        [Route("context/{contextId}/membership", Name = Constants.ServiceEndpoints.Nrps.MembershipService)]
        [Route("context/{contextId}/membership.{format}")]
        public async Task<ActionResult<MembershipContainer>> GetMembershipAsync([Required] string contextId,
            int? limit = null, string rlid = null, Role? role = null, bool groups = false)
        {
            var includeGroups = groups && await CallerHoldsGroupsScopeAsync().ConfigureAwait(false);
            var request = new GetMembershipRequest(contextId, limit, rlid, role, includeGroups) { CancellationToken = HttpContext.RequestAborted };
            var result = await OnGetMembershipAsync(request).ConfigureAwait(false);

            // Enforce, don't just hint: without the groups scope no group data leaves,
            // even if the override loaded it anyway.
            return includeGroups ? result : StripGroupEnrollments(result);
        }

        /// <summary>
        /// Whether the caller's token also grants the Course Groups Service scope.
        /// </summary>
        private async Task<bool> CallerHoldsGroupsScopeAsync()
        {
            var authorization = HttpContext.RequestServices.GetService<IAuthorizationService>();
            if (authorization == null) return false;

            try
            {
                var result = await authorization
                    .AuthorizeAsync(User, Constants.LtiScopes.Gs.ContextGroupReadonly)
                    .ConfigureAwait(false);
                return result.Succeeded;
            }
            catch (InvalidOperationException ex)
            {
                // Usually no policy for the scope (AddLtiAdvantagePolicies not called). Either way
                // the scope is not granted, but say why, or ?groups=true fails silently.
                _logger.LogWarning(ex,
                    "Could not check the {Scope} scope; serving membership without group enrollments.",
                    Constants.LtiScopes.Gs.ContextGroupReadonly);
                return false;
            }
        }

        /// <summary>
        /// Returns <paramref name="result"/> with group enrollments withheld. Works on copies, so
        /// containers and members the override shares or caches are left untouched.
        /// </summary>
        private static ActionResult<MembershipContainer> StripGroupEnrollments(ActionResult<MembershipContainer> result)
        {
            if (result.Value != null) return WithoutGroupEnrollments(result.Value);

            switch (result.Result)
            {
                case ObjectResult objectResult when objectResult.Value is MembershipContainer container:
                    objectResult.Value = WithoutGroupEnrollments(container);
                    break;
                case JsonResult jsonResult when jsonResult.Value is MembershipContainer container:
                    jsonResult.Value = WithoutGroupEnrollments(container);
                    break;
            }

            return result;
        }

        private static MembershipContainer WithoutGroupEnrollments(MembershipContainer container) => new()
        {
            Id = container.Id,
            Context = container.Context,
            Members = container.Members?.Select(m => m?.WithoutGroupEnrollments()).ToList()
        };
    }
}
