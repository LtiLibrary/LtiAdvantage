namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents a GetLineItems request.
    /// </summary>
    public class GetLineItemsRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public GetLineItemsRequest(string contextId, string ltiLinkId, string resourceId, string tag, int? limit)
        {
            ContextId = contextId;
            Limit = limit;
            LtiLinkId = ltiLinkId;
            ResourceId = resourceId;
            Tag = tag;
        }

        /// <summary>
        /// Get or set the context_id.
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// Get or set the limit.
        /// </summary>
        public int? Limit { get; }

        /// <summary>
        /// Get or set the lti_link_id.
        /// </summary>
        public string LtiLinkId { get; set; }

        /// <summary>
        /// Get or set the resource_id.
        /// </summary>
        public string ResourceId { get; set; }

        /// <summary>
        /// Get or set the tag.
        /// </summary>
        public string Tag { get; set; }
    }
}
