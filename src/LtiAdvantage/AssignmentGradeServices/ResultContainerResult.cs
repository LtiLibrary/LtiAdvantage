using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// ResultContainer JsonResult.
    /// </summary>
    public class ResultContainerResult : JsonResult
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the ResultContainerResult class.
        /// </summary>
        /// <param name="value">The ResultContainer to return.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public ResultContainerResult(ResultContainer value) : base(value)
        {
            ContentType = Constants.MediaTypes.ResultContainer;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the ResultContainerResult class
        /// with a status code. Typically used for non-success status codes.
        /// </summary>
        /// <param name="statusCode">The HTTP StatusCode to return.</param>
        public ResultContainerResult(int statusCode) : base(null)
        {
            ContentType = Constants.MediaTypes.ResultContainer;
            StatusCode = statusCode;
        }
    }
}
