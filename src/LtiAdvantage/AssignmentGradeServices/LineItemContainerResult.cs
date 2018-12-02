using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// LineItemContainer JsonResult.
    /// </summary>
    public class LineItemContainerResult : JsonResult
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the LineItemContainerResult class.
        /// </summary>
        /// <param name="value">The LineItemContainer to return.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public LineItemContainerResult(LineItemContainer value) : base(value)
        {
            ContentType = Constants.MediaTypes.LineItemContainer;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the LineItemContainerResult class
        /// with a status code. Typically used for non-success status codes.
        /// </summary>
        /// <param name="statusCode">The HTTP StatusCode to return.</param>
        public LineItemContainerResult(int statusCode) : base(null)
        {
            ContentType = Constants.MediaTypes.LineItemContainer;
            StatusCode = statusCode;
        }
    }
}
