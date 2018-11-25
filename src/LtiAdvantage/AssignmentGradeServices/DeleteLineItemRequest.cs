namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents a DeleteLineItem request.
    /// </summary>
    public class DeleteLineItemRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public DeleteLineItemRequest(string contextId, string id)
        {
            ContextId = contextId;
            Id = id;
        }

        /// <summary>
        /// Get or set the ContextId.
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// Get or set the Id.
        /// </summary>
        public string Id { get; }
    }
}
