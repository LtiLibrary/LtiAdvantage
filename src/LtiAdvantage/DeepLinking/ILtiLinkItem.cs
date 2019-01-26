using System.Collections.Generic;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// A link to an LTI resource, usually delivered by the same tool to
    /// which the deep linking request was made to.
    /// </summary>
    public interface ILtiLinkItem
    {
        /// <summary>
        /// </summary>
        StartEndProperty Available { get; set; }

        /// <summary>
        /// </summary>
        Dictionary<string, string> Custom { get; set; }

        /// <summary>
        /// </summary>
        ImageProperty Icon { get; set; }

        /// <summary>
        /// </summary>
        IframeProperty Iframe { get; set; }

        /// <summary>
        /// </summary>
        LineItemProperty LineItem { get; set; }

        /// <summary>
        /// </summary>
        StartEndProperty Submission { get; set; }

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
