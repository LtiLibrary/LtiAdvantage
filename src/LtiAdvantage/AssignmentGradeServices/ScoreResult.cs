using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LtiAdvantage.AssignmentGradeServices
{
    /// <inheritdoc />
    /// <summary>
    /// Score <see cref="JsonResult"/>.
    /// </summary>
    public class ScoreResult : JsonResult
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
            {NullValueHandling = NullValueHandling.Ignore};

        /// <inheritdoc />
        /// <summary>
        /// An empty Score <see cref="JsonResult"/> with specified status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code returned by the controller.</param>
        public ScoreResult(int statusCode) : base(null)
        {
            ContentType = Constants.MediaTypes.Score;
            StatusCode = statusCode;
        }

        /// <inheritdoc />
        /// <summary>
        /// A Score <see cref="JsonResult"/> with 201 Created status code.
        /// </summary>
        public ScoreResult(Score score) : base(score, Settings)
        {
            ContentType = Constants.MediaTypes.Score;
            StatusCode = StatusCodes.Status201Created;
        }
    }
}
