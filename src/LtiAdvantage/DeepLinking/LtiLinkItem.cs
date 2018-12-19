namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc cref="ILtiLinkItem" />
    /// <summary>
    /// A link to an LTI resource, usually delivered by the same tool to which the
    /// deep linking request was made to. 
    /// </summary>
    public class LtiLinkItem : ContentItem
    {
        /// <summary>
        /// Create a new content item.
        /// </summary>
        public LtiLinkItem()
        {
            Type = Constants.ContentItemTypes.LtiLink;
        }
    }
}
