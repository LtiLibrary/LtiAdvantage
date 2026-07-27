using System;
using System.Threading;
using System.Threading.Tasks;
using LtiAdvantage.AspNetCore.CourseGroupsService;
using LtiAdvantage.CourseGroupsService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace LtiAdvantage.IntegrationTests.CourseGroupsService
{
    /// <summary>
    /// The controller bases wrap the implementer's override in a catch-all that reports a 500
    /// with a ProblemDetails body. A cancelled request must not go down that path: the caller has
    /// already gone, so the 500 is written to a closed connection and, worse, shows up in logs
    /// and error budgets as a server fault rather than as an ordinary client disconnect.
    /// </summary>
    public class ControllerCancellationShould
    {
        [Fact]
        public async Task PropagateCancellation_WhenTheRequestWasAborted()
        {
            var controller = CreateController(requestAborted: true);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => controller.GetGroupsAsync("ctx-1"));
        }

        /// <summary>
        /// An OperationCanceledException that is not the caller leaving — an internal timeout,
        /// say — is still a fault, and must keep reporting as one.
        /// </summary>
        [Fact]
        public async Task StillReportAServerError_WhenTheRequestWasNotAborted()
        {
            var controller = CreateController(requestAborted: false);

            var result = await controller.GetGroupsAsync("ctx-1");

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.IsType<ProblemDetails>(objectResult.Value);
        }

        private static CancellingGroupsController CreateController(bool requestAborted)
        {
            var httpContext = new DefaultHttpContext();

            if (requestAborted)
            {
                var abort = new CancellationTokenSource();
                abort.Cancel();
                httpContext.RequestAborted = abort.Token;
            }

            return new CancellingGroupsController
            {
                ControllerContext = new ControllerContext {HttpContext = httpContext}
            };
        }

        /// <summary>
        /// A controller whose override always reports cancellation, whatever the request state.
        /// </summary>
        private sealed class CancellingGroupsController : GroupsControllerBase
        {
            public CancellingGroupsController()
                : base(new StubEnvironment(), NullLogger<GroupsControllerBase>.Instance)
            {
            }

            protected override Task<ActionResult<ContextGroupContainer>> OnGetGroupsAsync(GetGroupsRequest request)
            {
                throw new OperationCanceledException();
            }
        }

        private sealed class StubEnvironment : IWebHostEnvironment
        {
            public string EnvironmentName { get; set; } = "Production";
            public string ApplicationName { get; set; } = nameof(ControllerCancellationShould);
            public string WebRootPath { get; set; } = AppContext.BaseDirectory;
            public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
            public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }
    }
}
