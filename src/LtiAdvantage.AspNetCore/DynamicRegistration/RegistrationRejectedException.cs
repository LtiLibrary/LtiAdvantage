using System;
using Microsoft.AspNetCore.Http;

namespace LtiAdvantage.AspNetCore.DynamicRegistration
{
    /// <summary>
    /// Thrown from <c>OnRegisterAsync</c> to reject a registration. The endpoint answers with
    /// <see cref="StatusCode"/> (default 400) and a <see cref="RegistrationError"/> body
    /// (RFC 7591 §3.2.2, LTI Dynamic Registration 1.0 §3.6.2).
    /// </summary>
    public class RegistrationRejectedException : Exception
    {
        /// <summary>Rejects the registration.</summary>
        /// <param name="error">Error code, e.g. <c>invalid_client_metadata</c> or <c>invalid_redirect_uri</c>.</param>
        /// <param name="description">Optional human-readable description, sent to the tool.</param>
        /// <param name="statusCode">HTTP status code; must be a 4xx code. Default 400.</param>
        public RegistrationRejectedException(string error, string description, int statusCode = StatusCodes.Status400BadRequest)
            : base(string.IsNullOrWhiteSpace(description) ? $"Registration rejected: {error}." : description)
        {
            if (string.IsNullOrWhiteSpace(error)) throw new ArgumentException("An error code is required.", nameof(error));
            if (statusCode < 400 || statusCode > 499)
                throw new ArgumentOutOfRangeException(nameof(statusCode), statusCode, "Must be a 4xx status code.");

            Error = error;
            Description = string.IsNullOrWhiteSpace(description) ? null : description;
            StatusCode = statusCode;
        }

        /// <summary>The error code.</summary>
        public string Error { get; }

        /// <summary>The description sent to the tool, or <c>null</c> when none was given.</summary>
        public string Description { get; }

        /// <summary>The HTTP status code to answer with.</summary>
        public int StatusCode { get; }
    }
}
