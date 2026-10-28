using System.Threading;
using LtiAdvantage.AssignmentGradeServices;

namespace LtiAdvantage.AspNetCore.AssignmentGradeServices
{
    /// <summary>
    /// Represents a create item request request.
    /// </summary>
    public class AddLineItemRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public AddLineItemRequest(string contextId, LineItem lineItem)
        {
            ContextId = contextId;
            LineItem = lineItem;
        }

        /// <summary>
        /// The context id.
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// The line item.
        /// </summary>
        public LineItem LineItem { get; set; }

        /// <summary>
        /// Signalled when the caller disconnects (the request's <c>RequestAborted</c> token).
        /// Pass it to database and HTTP calls so work for a caller that has gone away stops.
        /// </summary>
        public CancellationToken CancellationToken { get; init; }
    }
}
