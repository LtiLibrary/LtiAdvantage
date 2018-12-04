using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// A LineItemContainer <see cref="JsonResult"/>.
    /// </summary>
    public class LineItemContainerResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {NullValueHandling = NullValueHandling.Ignore};

        /// <inheritdoc />
        /// <summary>
        /// An empty LineItemContainer <see cref="JsonResult"/> with specified status code.
        /// </summary>
        /// <param name="statusCode">The HTTP StatusCode to return.</param>
        public LineItemContainerResult(int statusCode) : base(null)
        {
            ContentType = Constants.MediaTypes.LineItemContainer;
            StatusCode = statusCode;
        }

        /// <inheritdoc />
        /// <summary>
        /// A LineItemContainer <see cref="JsonResult"/> with 200 status code.
        /// </summary>
        /// <param name="lineItemContainer">The LineItemContainer to return.</param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public LineItemContainerResult(LineItemContainer lineItemContainer) : base(lineItemContainer, Settings)
        {
            ContentType = Constants.MediaTypes.LineItemContainer;
            StatusCode = StatusCodes.Status200OK;
        }
    }
}
