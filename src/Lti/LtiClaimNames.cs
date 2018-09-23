namespace LtiAdvantageLibrary.NetCore.Lti
{
    /// <summary>
    /// LTI Advantage constants.
    /// </summary>
    public static class LtiClaimNames
    {
        /// <summary>
        /// Properties of the context from which the launch originated (for example, course id and title).
        /// </summary>
        public const string Context = "https://purl.imsglobal.org/spec/lti/claim/context";

        /// <summary>
        /// This is a map of key/value custom parameters which are to be included with the launch.
        /// </summary>
        public const string Custom = "https://purl.imsglobal.org/spec/lti/claim/custom";

        /// <summary>
        /// The deployment identifier, uniquely identifying the tool's deployment on the platform.
        /// </summary>
        public const string DeploymentId = "https://purl.imsglobal.org/spec/lti/claim/deployment_id";

        /// <summary>
        /// Information to help the Tool present itself appropriately.
        /// </summary>
        public const string LaunchPresentation = "https://purl.imsglobal.org/spec/lti/claim/launch_presentation";

        /// <summary>
        /// Properties about available Learning Information Services (LIS),
        /// usually originating from the Student Information System.
        /// </summary>
        public const string Lis = "https://purl.imsglobal.org/spec/lti/claim/lis";

        /// <summary>
        /// The type of message.
        /// </summary>
        public const string MessageType = "https://purl.imsglobal.org/spec/lti/claim/message_type";

        /// <summary>
        /// Properties associated with the platform initiating the launch.
        /// </summary>
        public const string Platform = "https://purl.imsglobal.org/spec/lti/claim/tool_platform";

        /// <summary>
        /// A link to tool's resource from the tool's platform.
        /// </summary>
        public const string ResourceLink = "https://purl.imsglobal.org/spec/lti/claim/resource_link";

        /// <summary>
        /// An array of roles as defined in the Core LTI specification.
        /// </summary>
        public const string Roles = "https://purl.imsglobal.org/spec/lti/claim/roles";

        /// <summary>
        /// An array of the user_id values which the current user can access as a mentor.
        /// </summary>
        public const string RoleScopeMentor = 
            "https://purl.imsglobal.org/spec/lti/claim/role_scope_mentor";

        /// <summary>
        /// The minimum LTI version required.
        /// </summary>
        public const string Version = "https://purl.imsglobal.org/spec/lti/claim/version";
    }
}
