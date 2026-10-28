using System.Threading;
using LtiAdvantage.Proctoring;

namespace LtiAdvantage.AspNetCore.Proctoring
{
    /// <summary>
    /// Represents an assessment control request.
    /// </summary>
    public class ControlAssessmentRequest
    {
        /// <summary>
        /// Initialize a new instance of the class.
        /// </summary>
        /// <param name="control">The submitted control action.</param>
        public ControlAssessmentRequest(AssessmentControlRequest control)
        {
            Control = control;
        }

        /// <summary>
        /// The control action the proctoring tool asked for.
        /// </summary>
        /// <remarks>
        /// Implementations should reject an action the deployment did not advertise in its
        /// <see cref="AssessmentControlClaimValueType.Actions"/>, and should verify that
        /// <see cref="AssessmentControlRequest.User"/> and
        /// <see cref="AssessmentControlRequest.AttemptNumber"/> identify an attempt the calling
        /// tool is actually proctoring.
        /// </remarks>
        public AssessmentControlRequest Control { get; }

        /// <summary>
        /// Signalled when the caller disconnects (the request's <c>RequestAborted</c> token).
        /// Pass it to database and HTTP calls so work for a caller that has gone away stops.
        /// </summary>
        public CancellationToken CancellationToken { get; init; }
    }
}
