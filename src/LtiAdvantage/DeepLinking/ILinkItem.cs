namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// A link is a fully qualified URL to a resource hosted on the internet.
    /// </summary>
    public interface ILinkItem
    {
        /// <summary>
        /// </summary>
        string Embed { get; set; }

        /// <summary>
        /// </summary>
        ImageProperty Icon { get; set; }

        /// <summary>
        /// </summary>
        IframeProperty Iframe { get; set; }

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
        WindowProperty Window { get; set; }
    }
}
