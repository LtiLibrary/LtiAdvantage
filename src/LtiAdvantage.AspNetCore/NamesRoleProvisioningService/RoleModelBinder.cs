using System.Threading.Tasks;
using LtiAdvantage.Lti;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LtiAdvantage.AspNetCore.NamesRoleProvisioningService
{
    /// <summary>
    /// Binds a <see cref="Role"/> query parameter from the full role URI the NRPS spec shows,
    /// the simple context role name (<c>Learner</c>), or the enum name (<c>ContextLearner</c>).
    /// </summary>
    /// <remarks>
    /// See https://www.imsglobal.org/spec/lti-nrps/v2p0#role-query-parameter.
    /// </remarks>
    public class RoleModelBinder : IModelBinder
    {
        /// <inheritdoc />
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (result == ValueProviderResult.None || string.IsNullOrEmpty(result.FirstValue))
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, result);
            if (RoleExtensions.TryParse(result.FirstValue, out var role))
            {
                bindingContext.Result = ModelBindingResult.Success(role);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                    $"'{result.FirstValue}' is not a known LTI role.");
            }

            return Task.CompletedTask;
        }
    }
}
