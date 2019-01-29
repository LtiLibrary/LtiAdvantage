using System.Threading.Tasks;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class ResultsController : ResultsControllerBase
    {
        public ResultsController(IHostingEnvironment env, ILogger<ResultsControllerBase> logger) : base(env, logger)
        {
        }

        protected override Task<ActionResult<ResultContainer>> OnGetResultsAsync(GetResultsRequest request)
        {
            var result = (ActionResult<ResultContainer>) new ObjectResult(new ResultContainer());
            return Task.FromResult(result);
        }
    }
}
