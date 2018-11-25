using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// A LineItem JsonResult.
    /// </summary>
    public class LineItemResult : JsonResult
    {
        /// <inheritdoc />
        /// <summary>
        /// Initialize a new instance of the LineItemResult with a line item.
        /// </summary>
        /// <param name="value">The line item."/&gt;</param>
        public LineItemResult(LineItem value) : base(value)
        {
            ContentType = Constants.MediaTypes.LineItem;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initialize a new instance of the LineItemResult without a line item.
        /// </summary>
        /// <param name="value">The object returned by the controller."/&gt;</param>
        /// <param name="statusCode">The HTTP status code returned by the controller.</param>
        public LineItemResult(int statusCode) : base(null)
        {
            ContentType = Constants.MediaTypes.LineItem;
            StatusCode = statusCode;
        }
    }
}
