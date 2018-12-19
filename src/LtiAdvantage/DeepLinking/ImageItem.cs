namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc cref="IImageItem" />
    /// <summary>
    /// Image is a URL pointing to an image resource that SHOULD be rendered
    /// directly in the browser agent using the HTML img tag.
    /// </summary>
    public class ImageItem : ContentItem
    {
        /// <summary>
        /// Create a new content item.
        /// </summary>
        public ImageItem()
        {
            Type = Constants.ContentItemTypes.Image;
        }
    }
}
