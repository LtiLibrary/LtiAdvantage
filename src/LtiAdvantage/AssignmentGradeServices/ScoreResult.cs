using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LtiAdvantage.AssignmentGradeServices
{
    public class ScoreResult : JsonResult
    {
        public ScoreResult(Score value) : base(value)
        {
            ContentType = Constants.MediaTypes.Score;
            StatusCode = StatusCodes.Status200OK;
        }

        public ScoreResult(Score value, int statusCode) : base(value)
        {
            ContentType = Constants.MediaTypes.Score;
            StatusCode = statusCode;
        }
    }
}
