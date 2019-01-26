namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// An HTML fragment to be embedded in html document.
    /// </summary>
    public interface IHtmlItem
    {
        /// <summary>
        /// </summary>
        string Html { get; set; }

        /// <summary>
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// </summary>
        string Title { get; set; }
    }
}
