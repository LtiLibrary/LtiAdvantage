using System.Collections.Generic;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// A list of LineItems.
    /// </summary>
    public class LineItemContainer : List<LineItem>
    {
        /// <inheritdoc />
        /// <summary>
        /// Create an empty LineItemContainer
        /// </summary>
        public LineItemContainer()
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Create a LineItemContainer that contains the line items.
        /// </summary>
        /// <param name="lineItems"></param>
        public LineItemContainer(List<LineItem> lineItems) : base(lineItems)
        {
        }
    }
}
