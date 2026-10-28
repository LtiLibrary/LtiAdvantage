using System.Threading;
namespace LtiAdvantage.AspNetCore.AssignmentGradeServices
{
    /// <summary>
    /// Represents a DeleteLineItem request.
    /// </summary>
    public class DeleteLineItemRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public DeleteLineItemRequest(string contextId, string lineItemId)
        {
            ContextId = contextId;
            LineItemId = lineItemId;
        }

        /// <summary>
        /// Get or set the ContextId.
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// Get or set the Id.
        /// </summary>
        public string LineItemId { get; }

        /// <summary>
        /// Signalled when the caller disconnects (the request's <c>RequestAborted</c> token).
        /// Pass it to database and HTTP calls so work for a caller that has gone away stops.
        /// </summary>
        public CancellationToken CancellationToken { get; init; }
    }
}
