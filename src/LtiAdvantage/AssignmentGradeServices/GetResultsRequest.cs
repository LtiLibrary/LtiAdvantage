namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents a GetResults request.
    /// </summary>
    public class GetResultsRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public GetResultsRequest(string contextId, string id, string userId)
        {
            ContextId = contextId;
            Id = id;
            UserId = userId;
        }

        /// <summary>
        /// Get or set the context_id.
        /// </summary>
        public string ContextId { get;  }
        
        /// <summary>
        /// Get or set the line item Id.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Get or set the user ID filter.
        /// </summary>
        public string UserId { get; }
    }
}
