using System;
using System.Net;
using System.Net.Http;

namespace LtiAdvantage.IdentityModel.Client
{
    /// <summary>
    /// A platform answered an LTI service call (AGS, NRPS, Course Groups) with an error status.
    /// </summary>
    /// <remarks>
    /// <see cref="Exception.Message"/> combines the status with what the platform said: the
    /// title and detail of an RFC 9457 ProblemDetails body (which LtiAdvantage platforms send),
    /// the <c>WWW-Authenticate</c> challenge when there is no body (a token without the scope),
    /// or the start of any other body.
    /// </remarks>
    public class LtiServiceException : HttpRequestException
    {
        /// <summary>Creates the exception.</summary>
        public LtiServiceException(string message, HttpStatusCode statusCode, string title, string detail,
            string problemType, string body, string wwwAuthenticate)
            : base(message, null, statusCode)
        {
            Title = title;
            Detail = detail;
            ProblemType = problemType;
            Body = body;
            WwwAuthenticate = wwwAuthenticate;
        }

        /// <summary>The ProblemDetails <c>title</c>, if the body was ProblemDetails.</summary>
        public string Title { get; }

        /// <summary>The ProblemDetails <c>detail</c>, if the body was ProblemDetails.</summary>
        public string Detail { get; }

        /// <summary>The ProblemDetails <c>type</c>, if the body was ProblemDetails.</summary>
        public string ProblemType { get; }

        /// <summary>The response body, cut to at most 4 KB.</summary>
        public string Body { get; }

        /// <summary>The <c>WWW-Authenticate</c> header, if any (it explains a 401 or 403 without a body).</summary>
        public string WwwAuthenticate { get; }
    }
}
