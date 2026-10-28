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

        protected override Task<ActionResult<MembershipContainer>> OnGetMembershipAsync(GetMembershipRequest request)
        {
            // One member; with group enrollments only when the base class passed IncludeGroups.
            var member = new Member {UserId = "user-1"};
            if (request.IncludeGroups)
                member.GroupEnrollments = new[] {new LtiAdvantage.CourseGroupsService.GroupEnrollment {GroupId = "group-1"}};

            var result = (ActionResult<MembershipContainer>) new ObjectResult(
                new MembershipContainer {Members = new List<Member> {member}});
            return Task.FromResult(result);
        }
    }
}
