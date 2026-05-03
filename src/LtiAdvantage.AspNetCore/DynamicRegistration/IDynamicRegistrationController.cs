using System.Threading.Tasks;
using LtiAdvantage.DynamicRegistration;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantage.AspNetCore.DynamicRegistration
{
    /// <summary>The platform-side LTI Dynamic Registration endpoint.</summary>
    public interface IDynamicRegistrationController
    {
        /// <summary>Registers a new tool. Returns 201 with the assigned <c>client_id</c> echoed back.</summary>
        Task<ActionResult<ToolConfiguration>> RegisterAsync([FromBody] ToolConfiguration tool);
    }
}
