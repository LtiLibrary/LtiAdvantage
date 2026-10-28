using System.Collections.Generic;
﻿using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.NamesRoleProvisioningService;
using LtiAdvantage.NamesRoleProvisioningService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class MembershipController : MembershipControllerBase
    {
        public MembershipController(IWebHostEnvironment env, ILogger<MembershipControllerBase> logger) : base(env, logger)
        {
        }

        /// <summary>A member shared between requests, as a cache would hand it out.</summary>
        public static readonly Member SharedMember = new()
        {
            UserId = "shared-user",
            GroupEnrollments = new[] {new LtiAdvantage.CourseGroupsService.GroupEnrollment {GroupId = "group-1"}}
        };

        protected override Task<ActionResult<MembershipContainer>> OnGetMembershipAsync(GetMembershipRequest request)
        {
            // Echo the role filter so tests can check how the query string was bound.
            Response.Headers["x-role"] = request.Role?.ToString() ?? "";

            // rlid "check-cancellation" answers 400 unless the request carries the abort token.
            if (request.Rlid == "check-cancellation" && !request.CancellationToken.CanBeCanceled)
                return Task.FromResult<ActionResult<MembershipContainer>>(new BadRequestResult());

            // rlid "shared-member" returns one cached Member that always has group enrollments, and
            // "json-result" returns it in a JsonResult, to check what the base class strips.
            if (request.Rlid == "shared-member" || request.Rlid == "json-result")
            {
                var shared = new MembershipContainer {Members = new List<Member> {SharedMember}};
                return Task.FromResult<ActionResult<MembershipContainer>>(request.Rlid == "json-result"
                    ? new JsonResult(shared)
                    : new ObjectResult(shared));
            }

            // One member; with group enrollments only when the base class passed IncludeGroups.
            var member = new Member {UserId = "user-1"};
            // rlid "always-groups" simulates an override that ignores IncludeGroups.
            if (request.IncludeGroups || request.Rlid == "always-groups")
                member.GroupEnrollments = new[] {new LtiAdvantage.CourseGroupsService.GroupEnrollment {GroupId = "group-1"}};

            var result = (ActionResult<MembershipContainer>) new ObjectResult(
                new MembershipContainer {Members = new List<Member> {member}});
            return Task.FromResult(result);
        }
    }
}
