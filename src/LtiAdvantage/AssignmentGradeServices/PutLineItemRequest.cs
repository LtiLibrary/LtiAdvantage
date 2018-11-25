namespace LtiAdvantage.AssignmentGradeServices
{
    /// <summary>
    /// Represents a PutLineItem DTO.
    /// </summary>
    public class PutLineItemRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        public PutLineItemRequest(LineItem lineItem)
        {
            LineItem = lineItem;
        }

        /// <summary>
        /// Get or set the LineItem.
        /// </summary>
        public LineItem LineItem { get; }
    }
}
