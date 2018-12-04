using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// ResultContainer JsonResult.
    /// </summary>
    public class ResultContainerResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {NullValueHandling = NullValueHandling.Ignore};

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
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the ResultContainerResult class.
        /// </summary>
        /// <param name="resultContainer">The ResultContainer to return.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public ResultContainerResult(ResultContainer resultContainer) : base(resultContainer, Settings)
        {
            ContentType = Constants.MediaTypes.ResultContainer;
            StatusCode = StatusCodes.Status200OK;
        }
    }
}
