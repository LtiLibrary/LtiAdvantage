using System;
using System.Collections.Generic;
using System.Text;
using LtiAdvantage.Lti;

namespace LtiAdvantage.AssignmentGradeServices
{
    public class GetLineItemsResponse : ServiceResponse
    {
        /// <summary>
        /// Get or set the LineItemContainer.
        /// </summary>
        public LineItemContainer LineItemContainer { get; set; }
    }
}
