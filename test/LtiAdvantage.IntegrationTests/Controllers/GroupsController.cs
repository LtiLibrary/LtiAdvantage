using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.CourseGroupsService;
using LtiAdvantage.CourseGroupsService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class GroupsController : GroupsControllerBase
    {
        public GroupsController(IWebHostEnvironment env, ILogger<GroupsControllerBase> logger) : base(env, logger)
        {
        }

        protected override Task<ActionResult<ContextGroupContainer>> OnGetGroupsAsync(GetGroupsRequest request)
        {
            // Echo the filters back so the tests can assert they were bound.
            var result = (ActionResult<ContextGroupContainer>) new ObjectResult(new ContextGroupContainer
            {
                Id = $"context/{request.ContextId}/groups",
                UserId = request.UserId,
                Groups = new[]
                {
                    new Group {Id = "group-1", Name = $"limit={request.Limit?.ToString() ?? "none"}"}
                }
            });

            return Task.FromResult(result);
        }
    }
}
