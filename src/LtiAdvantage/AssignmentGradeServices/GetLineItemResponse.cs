using LtiAdvantage.Lti;

namespace LtiAdvantage.AssignmentGradeServices
{
    public class GetLineItemResponse : ServiceResponse
    {
        /// <summary>
        /// Get or set the LineItem.
        /// </summary>
        public LineItem LineItem { get; set; }
    }
}