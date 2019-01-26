namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Image is a URL pointing to an image resource that SHOULD be rendered
    /// directly in the browser agent using the HTML img tag.
    /// </summary>
    public interface IImageItem
    {
        /// <summary>
        /// </summary>
        int? Height { get; set; }

        /// <summary>
        /// </summary>
        ImageProperty Icon { get; set; }

        /// <summary>
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// </summary>
        ImageProperty Thumbnail { get; set; }

        /// <summary>
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// </summary>
        int? Width { get; set; }
    }
}
