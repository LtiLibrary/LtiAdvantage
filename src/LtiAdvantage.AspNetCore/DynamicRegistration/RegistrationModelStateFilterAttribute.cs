using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LtiAdvantage.AspNetCore.DynamicRegistration
{
    /// <summary>
    /// Turns a missing or malformed registration request body into the RFC 7591 error body
    /// required by LTI Dynamic Registration 1.0 §3.6.2, instead of the default
    /// <see cref="ValidationProblemDetails"/> that <see cref="ApiControllerAttribute"/> returns.
    /// </summary>
    internal sealed class RegistrationModelStateFilterAttribute : ActionFilterAttribute
    {
        // Run before ApiController's ModelStateInvalidFilter (order -2000).
        public RegistrationModelStateFilterAttribute() => Order = -3000;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            var messages = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? e.Exception?.Message : e.ErrorMessage)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .Distinct();
            var description = string.Join(" ", messages);

            context.Result = new BadRequestObjectResult(RegistrationError.InvalidClientMetadata(
                string.IsNullOrWhiteSpace(description) ? "The request body is not a valid tool configuration." : description));
        }
    }
}
