using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.AssignmentGradeServices;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
#if NETCOREAPP2_1
using IWebHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class ResultsController : ResultsControllerBase
    {
        public ResultsController(IWebHostEnvironment env, ILogger<ResultsControllerBase> logger) : base(env, logger)
        {
        }

        protected override Task<ActionResult<ResultContainer>> OnGetResultsAsync(GetResultsRequest request)
        {
            var result = (ActionResult<ResultContainer>) new ObjectResult(new ResultContainer());
            return Task.FromResult(result);
        }
    }
}
