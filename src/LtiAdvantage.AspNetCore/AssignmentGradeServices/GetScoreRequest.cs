using System.Threading;
namespace LtiAdvantage.AspNetCore.AssignmentGradeServices
{
    /// <summary>
    /// Represents a Get Score request.
    /// </summary>
    public class GetScoreRequest
    {
        /// <summary>
        /// Create a GetScoreRequest.
        /// </summary>
        /// <param name="contextId">The context id.</param>
        /// <param name="lineItemId">The line item id.</param>
        /// <param name="scoreId">The score id.</param>
        public GetScoreRequest(string contextId, string lineItemId, string scoreId)
        {
            ContextId = contextId;
            LineItemId = lineItemId;
            ScoreId = scoreId;
        }

        /// <summary>
        /// Get or set the context id.
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// Get or set the line item id.
        /// </summary>
        public string LineItemId { get; set; }

        /// <summary>
        /// Get or set the score id.
        /// </summary>
        public string ScoreId { get; set; }

        /// <summary>
        /// Signalled when the caller disconnects (the request's <c>RequestAborted</c> token).
        /// Pass it to database and HTTP calls so work for a caller that has gone away stops.
        /// </summary>
        public CancellationToken CancellationToken { get; init; }
    }
}
