using System.Threading.Tasks;
using LtiAdvantage.AssignmentGradeServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class ScoresController : ScoresControllerBase
    {
        public ScoresController(IHostingEnvironment env, ILogger<ScoresControllerBase> logger) : base(env, logger)
        {
        }

        protected override Task<ActionResult<Score>> OnAddScoreAsync(AddScoreRequest request)
        {
            var result = (ActionResult<Score>) new ObjectResult(request.Score);
            return Task.FromResult(result);
        }

        protected override Task<ActionResult<Score>> OnGetScoreAsync(GetScoreRequest request)
        {
            var result = (ActionResult<Score>) new ObjectResult(new Score());
            return Task.FromResult(result);
        }
    }
}
