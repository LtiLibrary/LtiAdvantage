using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// Represents the <see cref="T:Microsoft.AspNetCore.Mvc.JsonResult" /> returned by the LineItemsController to the <see cref="T:LtiAdvantage.AssignmentGradeServices.LineItemsControllerBase" />
    /// </summary>
    public class LineItemResult : JsonResult
    {
        /// <summary>
        /// Initialize a new instance of the LineItemResult.
        /// </summary>
        /// <param name="value">The object returned by the controller."/></param>
        /// <param name="statusCode">The HTTP status code returned by the controller.</param>
        public LineItemResult(object value, int statusCode = StatusCodes.Status200OK) : base(value)
        {
            ContentType = Constants.MediaTypes.LineItem;
            StatusCode = statusCode;
        }
    }
}
