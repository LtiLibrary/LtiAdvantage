using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.Proctoring;
using LtiAdvantage.Proctoring;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.IntegrationTests.Controllers
{
    public class AssessmentControlController : AssessmentControlControllerBase
    {
        public AssessmentControlController(IWebHostEnvironment env,
            ILogger<AssessmentControlControllerBase> logger) : base(env, logger)
        {
        }

        protected override Task<ActionResult<AssessmentControlResponse>> OnControlAssessmentAsync(
            ControlAssessmentRequest request)
        {
            // Report the state the requested action would leave the attempt in.
            var status = request.Control.Action switch
            {
                AssessmentControlAction.Pause => AssessmentControlStatus.Paused,
                AssessmentControlAction.Resume => AssessmentControlStatus.Running,
                AssessmentControlAction.Terminate => AssessmentControlStatus.Terminated,
                _ => AssessmentControlStatus.Running
            };

            // Lets a test check that a response without a status is not sent as "status": null.
            if (request.Control.ReasonCode == "forget-status")
                status = AssessmentControlStatus.Unknown;

            var result = (ActionResult<AssessmentControlResponse>) new ObjectResult(new AssessmentControlResponse
            {
                Status = status,
                ExtraTime = request.Control.ExtraTime
            });

            return Task.FromResult(result);
        }
    }
}
