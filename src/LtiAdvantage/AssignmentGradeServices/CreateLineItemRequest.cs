namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents a create item request request.
    /// </summary>
    public class CreateLineItemRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public CreateLineItemRequest(string contextId, LineItem lineItem)
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
