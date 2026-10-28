using System;
using System.Collections.Generic;
using System.Threading;
using LtiAdvantage.AspNetCore.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Xunit;

namespace LtiAdvantage.IntegrationTests
{
    /// <summary>
    /// The service controller bases share one error contract through
    /// <see cref="ServiceExceptionFilterAttribute"/>: an unexpected exception becomes a logged 500
    /// with a ProblemDetails body. A cancelled request must not go down that path: the caller has
    /// already gone, so the 500 would be written to a closed connection and, worse, show up in
    /// logs and error budgets as a server fault rather than an ordinary client disconnect.
    /// </summary>
    public class ServiceExceptionFilterShould
    {
        [Fact]
        public void LeaveCancellationUnhandled_WhenTheRequestWasAborted()
        {
            var context = CreateContext(new OperationCanceledException(), requestAborted: true);

            new ServiceExceptionFilterAttribute().OnException(context);

            Assert.False(context.ExceptionHandled);
            Assert.Null(context.Result);
        }

        /// <summary>
        /// An OperationCanceledException that is not the caller leaving (an internal timeout, say)
        /// is still a fault, and must keep reporting as one.
        /// </summary>
        [Fact]
        public void ReportAServerError_ForACancellationThatIsNotTheCallerLeaving()
        {
            var context = CreateContext(new OperationCanceledException(), requestAborted: false);

            new ServiceExceptionFilterAttribute().OnException(context);

            AssertProblem(context, expectDetail: false);
        }

        [Theory]
        [InlineData("Production", false)]
        [InlineData("Development", true)]
        public void IncludeExceptionDetails_OnlyInDevelopment(string environment, bool expectDetail)
        {
            var context = CreateContext(new InvalidOperationException("Login failed for user X on server Y"),
                requestAborted: false, environment);

            new ServiceExceptionFilterAttribute().OnException(context);

            var problem = AssertProblem(context, expectDetail);
            if (expectDetail) Assert.Contains("server Y", problem.Detail);
        }

        private static ProblemDetails AssertProblem(ExceptionContext context, bool expectDetail)
        {
            Assert.True(context.ExceptionHandled);
            var result = Assert.IsType<ObjectResult>(context.Result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            var problem = Assert.IsType<ProblemDetails>(result.Value);
            Assert.Equal(expectDetail, problem.Detail != null);
            return problem;
        }

        private static ExceptionContext CreateContext(Exception exception, bool requestAborted,
            string environment = "Production")
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IWebHostEnvironment>(new StubEnvironment {EnvironmentName = environment})
                .BuildServiceProvider();

            var httpContext = new DefaultHttpContext {RequestServices = services};
            if (requestAborted)
            {
                var abort = new CancellationTokenSource();
                abort.Cancel();
                httpContext.RequestAborted = abort.Token;
            }

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ExceptionContext(actionContext, new List<IFilterMetadata>()) {Exception = exception};
        }

        private sealed class StubEnvironment : IWebHostEnvironment
        {
            public string EnvironmentName { get; set; } = "Production";
            public string ApplicationName { get; set; } = nameof(ServiceExceptionFilterShould);
            public string WebRootPath { get; set; } = AppContext.BaseDirectory;
            public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
            public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }
    }
}
