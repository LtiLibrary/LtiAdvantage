namespace LtiAdvantage.DeepLinking
{
    /// <inheritdoc cref="IFileItem" />
    /// <summary>
    /// A file is a resource transferred from the tool to stored and/or processed by the platform.
    /// </summary>
    public class FileItem : ContentItem
    {
        /// <summary>
        /// Create a new content item.
        /// </summary>
        public FileItem()
        {
            Type = Constants.ContentItemTypes.File;
        }
    }
}
