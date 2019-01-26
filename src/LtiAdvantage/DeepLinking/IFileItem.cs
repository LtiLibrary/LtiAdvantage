using System;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// A file is a resource transferred from the tool to stored and/or processed by the platform.
    /// </summary>
    public interface IFileItem
    {
        /// <summary>
        /// </summary>
        DateTime? ExpiresAt { get; set; }

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
    }
}
