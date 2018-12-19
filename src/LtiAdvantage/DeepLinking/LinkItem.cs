namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc cref="ILinkItem" />
    /// <summary>
    /// A link is a fully qualified URL to a resource hosted on the internet.
    /// </summary>
    public class LinkItem : ContentItem
    {
        /// <summary>
        /// Create a new content item.
        /// </summary>
        public LinkItem()
        {
            Type = Constants.ContentItemTypes.Link;
        }
    }
}
