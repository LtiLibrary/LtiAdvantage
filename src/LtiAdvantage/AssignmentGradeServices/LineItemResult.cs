using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// A LineItem <see cref="JsonResult"/>.
    /// </summary>
    public class LineItemResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {NullValueHandling = NullValueHandling.Ignore};

        /// <inheritdoc />
        /// <summary>
        /// Empty LineItem <see cref="JsonResult"/> with specified status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code returned by the controller.</param>
        public LineItemResult(int statusCode) : base(null, Settings)
        {
            ContentType = Constants.MediaTypes.LineItem;
            StatusCode = statusCode;
        }

        /// <inheritdoc />
        /// <summary>
        /// LineItem <see cref="JsonResult"/> with 200 status code.
        /// </summary>
        /// <param name="lineItem">The line item."</param>
        public LineItemResult(LineItem lineItem) : base(lineItem, Settings)
        {
            ContentType = Constants.MediaTypes.LineItem;
            StatusCode = StatusCodes.Status200OK;
        }

        /// <inheritdoc />
        /// <summary>
        /// LineItem <see cref="JsonResult"/> with specified status code.
        /// </summary>
        /// <param name="lineItem">The line item."</param>
        /// <param name="statusCode">The HTTP status code returned by the controller.</param>
        public LineItemResult(LineItem lineItem, int statusCode) : base(lineItem, Settings)
        {
            ContentType = Constants.MediaTypes.LineItem;
            StatusCode = statusCode;
        }
    }
}
