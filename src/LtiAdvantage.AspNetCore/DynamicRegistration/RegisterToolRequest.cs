using LtiAdvantage.DynamicRegistration;

namespace LtiAdvantage.AspNetCore.DynamicRegistration
{
    /// <summary>Request passed to the controller's <c>OnRegisterAsync</c> override.</summary>
    public class RegisterToolRequest
    {
        /// <summary>Wraps the submitted tool configuration.</summary>
        /// <param name="tool">The submitted tool configuration.</param>
        public RegisterToolRequest(ToolConfiguration tool) => Tool = tool;

        /// <summary>The submitted tool configuration. The override should populate <see cref="ToolConfiguration.ClientId"/>.</summary>
        public ToolConfiguration Tool { get; }
    }
}
