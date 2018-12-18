using LtiAdvantage.Lti;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc />
    /// <summary>
    /// LTI Deep Linking request.
    /// </summary>
    public class LtiDeepLinkingRequest : LtiRequest
    {
        /// <inheritdoc />
        /// <summary>
        /// Create an empty request.
        /// </summary>
        public LtiDeepLinkingRequest()
        {
            MessageType = Constants.Lti.LtiDeepLinkingRequestMessageType;
            Version = Constants.Lti.Version;
        }

        /// <summary>
        /// Deep Linking settings.
        /// </summary>
        public DeepLinkingSettingsClaimValueType DeepLinkingSettings
        {
            get { return this.GetClaimValue<DeepLinkingSettingsClaimValueType>(Constants.LtiClaims.DeepLinkingSettings); }
            set { this.SetClaimValue(Constants.LtiClaims.DeepLinkingSettings, value);}
        }
    }
}
