namespace LtiAdvantage
{
    /// <summary>
    /// LTI Advantage Library constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// LTI Advantage constants.
        /// </summary>
        public static class Lti
        {
            /// <summary>
            /// The message type of an LtiResourceLinkRequest.
            /// </summary>
            public const string LtiResourceLinkRequestMessageType = "LtiResourceLinkRequest";

            /// <summary>
            /// LTI version.
            /// </summary>
            public const string Version = "1.3.0";
        }

        /// <summary>
        /// LTI claims.
        /// </summary>
        public static class LtiClaims
        {
            /// <summary>
            /// The claim to include Names and Role Provisioning Service parameter in LTI 1.3 messages.
            /// </summary>
            public const string AssignmentGradeServices = "https://purl.imsglobal.org/spec/lti-ags/claim/endpoint";

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
            /// User ID as defined in LTI 1.1.
            /// </summary>
            public const string Lti11LegacyUserId = "https://purl.imsglobal.org/spec/lti/claim/lti11_legacy_user_id";

            /// <summary>
            /// The type of message.
            /// </summary>
            public const string MessageType = "https://purl.imsglobal.org/spec/lti/claim/message_type";

            /// <summary>
            /// The claim to include Names and Role Provisioning Service parameter in LTI 1.3 messages.
            /// </summary>
            public const string NamesRoleService = "https://purl.imsglobal.org/spec/lti-nrps/claim/namesroleservice";

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
            public const string RoleScopeMentor = "https://purl.imsglobal.org/spec/lti/claim/role_scope_mentor";

            /// <summary>
            /// The launch url.
            /// </summary>
            public const string TargetLinkUri = "https://purl.imsglobal.org/spec/lti/claim/target_link_uri";

            /// <summary>
            /// The minimum LTI version required.
            /// </summary>
            public const string Version = "https://purl.imsglobal.org/spec/lti/claim/version";
        }

        /// <summary>
        /// LTI Advantage API scopes.
        /// </summary>
        public static class LtiScopes
        {
            /// <summary>
            /// Assignment and Grade Service lineitem read/write scope.
            /// </summary>
            public const string AgsLineItem = "https://purl.imsglobal.org/spec/lti-ags/scope/lineitem";

            /// <summary>
            /// Assignment and Grade Service lineitem readonly scope.
            /// </summary>
            public const string AgsLineItemReadonly = "https://purl.imsglobal.org/spec/lti-ags/scope/lineitem.readonly";

            /// <summary>
            /// Assignment and Grade Service result readonly scope.
            /// </summary>
            public const string AgsResultReadonly = "https://purl.imsglobal.org/spec/lti-ags/scope/result.readonly";

            /// <summary>
            /// Assignment and Grade Service score read/write scope.
            /// </summary>
            public const string AgsScore = "https://purl.imsglobal.org/spec/lti-ags/scope/score";

            /// <summary>
            /// Custom Assignment and Grade Service score readonly scope.
            /// </summary>
            public const string AgsScoreReadonly = "https://purl.imsglobal.org/spec/lti-ags/scope/score.readonly";

            /// <summary>
            /// Names and Role Service readonly scope.
            /// </summary>
            public const string NrpsMembershipReadonly = "https://purl.imsglobal.org/spec/lti-nrps/scope/contextmembership.readonly";
        }

        /// <summary>
        /// Service media types.
        /// </summary>
        public static class MediaTypes
        {
            /// <summary>
            /// https://www.imsglobal.org/spec/lti-ags/v2p0/#media-types-and-schemas
            /// </summary>
            public const string LineItem = "application/vnd.ims.lis.v2.lineitem+json";

            /// <summary>
            /// https://www.imsglobal.org/spec/lti-ags/v2p0/#media-types-and-schemas
            /// </summary>
            public const string LineItemContainer = "application/vnd.ims.lis.v2.lineitemcontainer+json";

            /// <summary>
            /// https://www.imsglobal.org/spec/lti-nrps/v2p0#membership-container-media-type
            /// </summary>
            public const string MembershipContainer = "application/vnd.ims.lis.v2.membershipcontainer+json";

            /// <summary>
            /// https://www.imsglobal.org/spec/lti-ags/v2p0/#media-types-and-schemas
            /// </summary>
            public const string ResultContainer = "application/vnd.ims.lis.v2.resultcontainer+json";

            /// <summary>
            /// https://www.imsglobal.org/spec/lti-ags/v2p0/#media-types-and-schemas
            /// </summary>
            public const string Score = "application/vnd.ims.lis.v1.score+json";
        }

        /// <summary>
        /// Subset of OpenID standard claims used by LTI Advantage.
        /// See https://openid.net/specs/openid-connect-core-1_0.html#Claims.
        /// </summary>
        public static class OidcClaims
        {
            /// <summary>
            /// End-User's preferred e-mail address. Its value MUST conform to the RFC 5322 [RFC5322] addr-spec
            /// syntax. The RP MUST NOT rely upon this value being unique.
            /// </summary>
            public const string Email = "email";

            /// <summary>
            /// Surname(s) or last name(s) of the End-User. Note that in some cultures, people can have multiple
            /// family names or no family name; all can be present, with the names being separated by space characters.
            /// </summary>
            public const string FamilyName = "family_name";

            /// <summary>
            /// Given name(s) or first name(s) of the End-User. Note that in some cultures, people can have multiple
            /// given names; all can be present, with the names being separated by space characters.
            /// </summary>
            public const string GivenName = "given_name";

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
            public const string Locale = "locale";

            /// <summary>
            /// Middle name(s) of the End-User. Note that in some cultures, people can have multiple middle
            /// names; all can be present, with the names being separated by space characters. Also note that
            /// in some cultures, middle names are not used.
            /// <example>
            /// "Marie"
            /// </example>
            /// </summary>
            public const string MiddleName = "middle_name";

            /// <summary>
            /// End-User's full name in displayable form including all name parts, possibly including titles and
            /// suffixes, ordered according to the End-User's locale and preferences.
            /// <example>
            /// "Ms Jane Marie Doe"
            /// </example>
            /// </summary>
            public const string Name = "name";

            /// <summary>
            /// URL of the End-User's profile picture. This URL MUST refer to an image file (for example, a PNG,
            /// JPEG, or GIF image file), rather than to a Web page containing an image. Note that this URL SHOULD
            /// specifically reference a profile photo of the End-User suitable for displaying when describing the
            /// End-User, rather than an arbitrary photo taken by the End-User.
            /// <example>
            /// "http://example.org/jane.jpg"
            /// </example>
            /// </summary>
            public const string Picture = "picture";
        }

        /// <summary>
        /// LTI Advantage service endpoints.
        /// </summary>
        public static class ServiceEndpoints
        {
            /// <summary>
            /// Assignment and Grade Services line item endpoint.
            /// </summary>
            public const string AgsLineItemService = "lineitem";

            /// <summary>
            /// Assignment and Grade Services line items endpoint.
            /// </summary>
            public const string AgsLineItemsService = "lineitems";

            /// <summary>
            /// Assignment and Grade Services results endpoint.
            /// </summary>
            public const string AgsResultsService = "results";

            /// <summary>
            /// Assignment and Grade Services score endpoint.
            /// </summary>
            public const string AgsScoreService = "score";

            /// <summary>
            /// Assignment and Grade Services scores endpoint.
            /// </summary>
            public const string AgsScoresService = "scores";

            /// <summary>
            /// Names and Role Provisioning Service membership endpoint.
            /// </summary>
            public const string NrpsMembershipService = "membership";
        }
    }
}
