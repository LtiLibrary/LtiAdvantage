using System.Threading;
using System;

namespace LtiAdvantage.AspNetCore.CourseGroupsService
{
    /// <summary>
    /// Represents a GetGroupSets request.
    /// </summary>
    public class GetGroupSetsRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        /// <param name="contextId">Required context id.</param>
        /// <param name="limit">Optional limit to the number of group sets to return.</param>
        public GetGroupSetsRequest(string contextId, int? limit = null)
        {
            ContextId = contextId ?? throw new ArgumentNullException(nameof(contextId));
            Limit = limit;
        }

        /// <summary>
        /// Required context ID of the context whose group sets to return. For example, the course.
        /// </summary>
        public string ContextId { get; }

        /// <summary>
        /// Specifies the maximum number of group sets to return.
        /// </summary>
        /// <remarks>
        /// This parameter is merely a hint. The server is not obligated to honor this
        /// limit and may at its own discretion choose a different value for the number
        /// of group sets to return.
        /// See https://www.imsglobal.org/spec/lti-gs/v1p0#limit-query-parameter.
        /// </remarks>
        public int? Limit { get; }

        /// <summary>
        /// Signalled when the caller disconnects (the request's <c>RequestAborted</c> token).
        /// Pass it to database and HTTP calls so work for a caller that has gone away stops.
        /// </summary>
        public CancellationToken CancellationToken { get; init; }
    }
}
