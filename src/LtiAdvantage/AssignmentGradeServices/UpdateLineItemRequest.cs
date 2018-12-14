namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents an update line item request.
    /// </summary>
    public class UpdateLineItemRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public UpdateLineItemRequest(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        /// <summary>
        /// Get or set the LineItem.
        /// </summary>
        public LineItem LineItem { get; }
    }
}
