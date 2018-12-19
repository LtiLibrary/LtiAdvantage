namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc cref="IHtmlItem" />
    /// <summary>
    /// An HTML fragment to be embedded in html document.
    /// </summary>
    public class HtmlItem : ContentItem
    {
        /// <summary>
        /// Create a new content item.
        /// </summary>
        public HtmlItem()
        {
            Type = Constants.ContentItemTypes.Html;
        }
    }
}
