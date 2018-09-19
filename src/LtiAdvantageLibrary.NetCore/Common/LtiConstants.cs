namespace LtiAdvantageLibrary.NetCore.Common
{
    /// <summary>
    /// LTI Advantage constants.
    /// </summary>
    public static class LtiConstants
    {
        /// <summary>
        /// Properties of the context from which the launch originated (for example, course id and title).
        /// </summary>
        public const string ContextClaim = "https://purl.imsglobal.org/spec/lti/claim/context";

        /// <summary>
        /// This is a map of key/value custom parameters which are to be included with the launch.
        /// </summary>
        public const string CustomClaim = "https://purl.imsglobal.org/spec/lti/claim/custom";

        /// <summary>
        /// The deployment identifier, uniquely identifying the tool's deployment on the platform.
        /// </summary>
        public const string DeploymentIdClaim = "https://purl.imsglobal.org/spec/lti/claim/deployment_id";

        /// <summary>
        /// End-User's preferred e-mail address. Its value MUST conform to the RFC 5322 [RFC5322]
        /// addr-spec syntax. The Tool MUST NOT rely upon this value being unique.
        /// <example>
        /// "jane@example.org"
        /// </example>
        /// </summary>
        public const string EmailClaim = "email";

        /// <summary>
        /// Surname(s) or last name(s) of the End-User. Note that in some cultures, people can have
        /// multiple family names or no family name; all can be present, with the names being separated
        /// by space characters.
        /// <example>
        /// "Doe"
        /// </example>
        /// </summary>
        public const string FamilyNameClaim = "family_name";

        /// <summary>
        /// Given name(s) or first name(s) of the End-User. Note that in some cultures, people can have
        /// multiple given names; all can be present, with the names being separated by space characters.
        /// <example>
        /// "Jane"
        /// </example>
        /// </summary>
        public const string GivenNameClaim = "given_name";

        /// <summary>
        /// Information to help the Tool present itself appropriately.
        /// </summary>
        public const string LaunchPresentationClaim = 
            "https://purl.imsglobal.org/spec/lti/claim/launch_presentation";

        /// <summary>
        /// Properties about available Learning Information Services (LIS),
        /// usually originating from the Student Information System.
        /// </summary>
        public const string LisClaim = "https://purl.imsglobal.org/spec/lti/claim/lis";

        /// <summary>
        /// The message type of an LtiResourceLinkRequest.
        /// </summary>
        public const string LtiResourceLinkRequestMessageType = "LtiResourceLinkRequest";

        /// <summary>
        /// End-User's locale, represented as a BCP47 [RFC5646] language tag. This is typically an
        /// ISO 639-1 Alpha-2 [ISO639‑1] language code in lowercase and an ISO 3166-1 Alpha-2 [ISO3166‑1]
        /// country code in uppercase, separated by a dash. For example, en-US or fr-CA. As a compatibility
        /// note, some implementations have used an underscore as the separator rather than a dash, for
        /// example, en_US; Tools MAY choose to accept this locale syntax as well.
        /// <example>
        /// en-US
        /// </example>
        /// </summary>
        public const string LocaleClaim = "locale";

        /// <summary>
        /// The type of message.
        /// </summary>
        public const string MessageTypeClaim = "https://purl.imsglobal.org/spec/lti/claim/message_type";

        /// <summary>
        /// Middle name(s) of the End-User. Note that in some cultures, people can have multiple middle
        /// names; all can be present, with the names being separated by space characters. Also note that
        /// in some cultures, middle names are not used.
        /// <example>
        /// "Marie"
        /// </example>
        /// </summary>
        public const string MiddleNameClaim = "middle_name";

        /// <summary>
        /// End-User's full name in displayable form including all name parts, possibly including titles and
        /// suffixes, ordered according to the End-User's locale and preferences.
        /// <example>
        /// "Ms Jane Marie Doe"
        /// </example>
        /// </summary>
        public const string NameClaim = "name";

        /// <summary>
        /// URL of the End-User's profile picture. This URL MUST refer to an image file (for example, a PNG,
        /// JPEG, or GIF image file), rather than to a Web page containing an image. Note that this URL SHOULD
        /// specifically reference a profile photo of the End-User suitable for displaying when describing the
        /// End-User, rather than an arbitrary photo taken by the End-User.
        /// <example>
        /// "http://example.org/jane.jpg"
        /// </example>
        /// </summary>
        public const string PictureClaim = "picture";

        /// <summary>
        /// Properties associated with the platform initiating the launch.
        /// </summary>
        public const string PlatformClaim = "https://purl.imsglobal.org/spec/lti/claim/tool_platform";

        /// <summary>
        /// A link to tool's resource from the tool's platform.
        /// </summary>
        public const string ResourceLinkClaim = "https://purl.imsglobal.org/spec/lti/claim/resource_link";

        /// <summary>
        /// An array of roles as defined in the Core LTI specification.
        /// </summary>
        public const string RolesClaim = "https://purl.imsglobal.org/spec/lti/claim/roles";

        /// <summary>
        /// An array of the user_id values which the current user can access as a mentor.
        /// </summary>
        public const string RoleScopeMentorClaim = 
            "https://purl.imsglobal.org/spec/lti/claim/role_scope_mentor";

        /// <summary>
        /// The minimum LTI version required.
        /// </summary>
        public const string VersionClaim = "https://purl.imsglobal.org/spec/lti/claim/version";
    }
}
