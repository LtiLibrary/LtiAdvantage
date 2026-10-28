using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LtiAdvantage.AspNetCore.Utilities
{
    /// <summary>
    /// The error contract shared by the LTI service controller bases: an exception thrown by an
    /// action (including the implementer's <c>On…Async</c> override) is logged and answered with a
    /// 500 <see cref="ProblemDetails"/>. Exception details are only included in Development,
    /// because messages can reveal configuration (connection strings, host names, paths).
    /// </summary>
    /// <remarks>
    /// A cancellation caused by the client disconnecting is not handled: it propagates, so the
    /// framework aborts the response instead of logging a server error nobody will see.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public sealed class ServiceExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            if (exception is OperationCanceledException && context.HttpContext.RequestAborted.IsCancellationRequested)
            {
                return;
            }

            var services = context.HttpContext.RequestServices;
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            var category = descriptor?.ControllerTypeInfo.FullName ?? typeof(ServiceExceptionFilterAttribute).FullName;
            services.GetService<ILoggerFactory>()?.CreateLogger(category)
                .LogError(exception, "An unexpected error occurred in {Action}.",
                    descriptor?.ActionName ?? context.ActionDescriptor.DisplayName);

            var isDevelopment = services.GetService<IWebHostEnvironment>()?.IsDevelopment() == true;
            context.Result = new ObjectResult(new ProblemDetails
            {
                Title = "An unexpected error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Detail = isDevelopment ? exception.Message + exception.StackTrace : null
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
            context.ExceptionHandled = true;
        }
    }
}
