using System.Threading;
using System;

namespace LtiAdvantage.AspNetCore.CourseGroupsService
{
    /// <summary>
    /// Represents a GetGroups request.
    /// </summary>
    public class GetGroupsRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        /// <param name="contextId">Required context id.</param>
        /// <param name="userId">Optional user filter for groups the user belongs to.</param>
        /// <param name="limit">Optional limit to the number of groups to return.</param>
        public GetGroupsRequest(string contextId, string userId = null, int? limit = null)
        {
            ContextId = contextId ?? throw new ArgumentNullException(nameof(contextId));
            UserId = userId;
            Limit = limit;
        }

        /// <summary>
        /// Required context ID of the context whose groups to return. For example, the course.
        /// </summary>
        public string ContextId { get; }

        /// <summary>
        /// Optional user filter for groups to return.
        /// </summary>
        /// <remarks>
        /// The result set will be filtered so that it includes only the groups the specified
        /// user is a member of. If omitted, all groups in the context are returned.
        /// See https://www.imsglobal.org/spec/lti-gs/v1p0#user-id-query-parameter.
        /// </remarks>
        public string UserId { get; }

        /// <summary>
        /// Specifies the maximum number of groups to return.
        /// </summary>
        /// <remarks>
        /// This parameter is merely a hint. The server is not obligated to honor this
        /// limit and may at its own discretion choose a different value for the number
        /// of groups to return.
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
