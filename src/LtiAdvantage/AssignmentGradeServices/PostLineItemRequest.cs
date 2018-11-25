namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents a PostLineItem request.
    /// </summary>
    public class PostLineItemRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public PostLineItemRequest(string contextId, LineItem lineItem)
        {
            ContextId = contextId;
            LineItem = lineItem;
        }

        /// <summary>
        /// The ContextId (course) for this <see cref="LineItem"/>.
        /// </summary>
        public string ContextId { get; set; }

        /// <summary>
        /// The <see cref="LineItem"/>.
        /// </summary>
        public LineItem LineItem { get; set; }
    }
}
