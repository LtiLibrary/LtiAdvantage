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
        /// An empty ResultContainer <see cref="JsonResult"/> with specified status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code returned by the controller.</param>
        public ResultContainerResult(int statusCode) : base(null)
        {
            ContentType = Constants.MediaTypes.ResultContainer;
            StatusCode = statusCode;
        }

        /// <inheritdoc />
        /// <summary>
        /// A ResultContainer <see cref="JsonResult"/> with 200 status code.
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
