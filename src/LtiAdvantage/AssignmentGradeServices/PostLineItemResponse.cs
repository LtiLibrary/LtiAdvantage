using LtiAdvantage.Lti;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// Response to posting lineitem.
    /// </summary>
    public class PostLineItemResponse : ServiceResponse
    {
        /// <summary>
        /// Get or set the lineitem.
        /// </summary>
        public LineItem LineItem { get; set; }
    }
}
