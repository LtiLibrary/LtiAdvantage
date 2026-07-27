using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.CourseGroupsService;
using LtiAdvantage.CourseGroupsService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class GroupSetsController : GroupSetsControllerBase
    {
        public GroupSetsController(IWebHostEnvironment env, ILogger<GroupSetsControllerBase> logger) : base(env, logger)
        {
        }

        protected override Task<ActionResult<ContextGroupSetContainer>> OnGetGroupSetsAsync(GetGroupSetsRequest request)
        {
            var result = (ActionResult<ContextGroupSetContainer>) new ObjectResult(new ContextGroupSetContainer
            {
                Id = $"context/{request.ContextId}/groups/sets",
                Sets = new[]
                {
                    new GroupSet {Id = "set-1", Name = "Lab Groups"}
                }
            });

            return Task.FromResult(result);
        }
    }
}
