namespace LtiAdvantage
{
    /// <summary>
    /// LTI Advantage Library constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Deep linking content item types.
        /// </summary>
        public static class ContentItemTypes
        {
            /// <summary>
            /// A file is a resource transferred from the tool to stored and/or processed
            /// by the platform.
            /// </summary>
            public const string File = "file";

            /// <summary>
            /// An HTML fragment to be embedded in html document.
            /// </summary>
            public const string Html = "html";

            /// <summary>
            /// Image is a URL pointing to an image resource that SHOULD be rendered directly
            /// in the browser agent using the HTML img tag.
            /// </summary>
            public const string Image = "image";

            /// <summary>
            /// A link is a fully qualified URL to a resource hosted on the internet.
            /// </summary>
            public const string Link = "link";

            /// <summary>
            /// A link to an LTI resource, usually delivered by the same tool to which
            /// the deep linking request was made to.
            /// </summary>
            public const string LtiLink = "ltiResourceLink";
        }

        /// <summary>
        /// LTI Advantage constants.
        /// </summary>
        public static class Lti
        {
            /// <summary>
            /// The message type of an LtiDeepLinkingRequest.
            /// </summary>
            public const string LtiDeepLinkingRequestMessageType = "LtiDeepLinkingRequest";

            /// <summary>
            /// The message type of an LtiDeepLinkingRequest.
            /// </summary>
            public const string LtiDeepLinkingResponseMessageType = "LtiDeepLinkingResponse";

            /// <summary>
            /// The message type of an LtiEndAssessmentRequest (Proctoring Services 1.0 §4.4).
            /// </summary>
            public const string LtiEndAssessmentRequestMessageType = "LtiEndAssessment";

            /// <summary>
            /// The message type of an LtiResourceLinkRequest.
            /// </summary>
            public const string LtiResourceLinkRequestMessageType = "LtiResourceLinkRequest";

            /// <summary>
            /// The message type of an LtiStartAssessmentRequest (Proctoring Services 1.0 §4.3).
            /// </summary>
            public const string LtiStartAssessmentRequestMessageType = "LtiStartAssessment";

            /// <summary>
            /// The message type of an LtiStartProctoringRequest (Proctoring Services 1.0 §4.2).
            /// </summary>
            public const string LtiStartProctoringRequestMessageType = "LtiStartProctoring";

            /// <summary>
            /// The message type of an LtiSubmissionReviewRequest.
            /// </summary>
            public const string LtiSubmissionReviewRequestMessageType = "LtiSubmissionReviewRequest";

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
            /// The Assessment Control Service endpoint and permitted actions (Proctoring Services 1.0 §5.2).
            /// </summary>
            public const string Acs = "https://purl.imsglobal.org/spec/lti-ap/claim/acs";

            /// <summary>
            /// The claim to include Names and Role Provisioning Service parameter in LTI 1.3 messages.
            /// </summary>
            public const string AssignmentGradeServices = "https://purl.imsglobal.org/spec/lti-ags/claim/endpoint";

            /// <summary>
            /// The attempt number of the assessment being proctored (Proctoring Services 1.0 §4.2).
            /// </summary>
            public const string AttemptNumber = "https://purl.imsglobal.org/spec/lti-ap/claim/attempt_number";

            /// <summary>
            /// A list of content items.
            /// </summary>
            public const string ContentItems = "https://purl.imsglobal.org/spec/lti-dl/claim/content_items";

            /// <summary>
            /// Properties of the context from which the launch originated (for example, course id and title).
            /// </summary>
            public const string Context = "https://purl.imsglobal.org/spec/lti/claim/context";

            /// <summary>
            /// The claim to include Course Groups Service parameters in LTI 1.3 messages
            /// (Course Groups Service 1.0 §2.1).
            /// </summary>
            public const string CourseGroupsService = "https://purl.imsglobal.org/spec/lti-gs/claim/groupsservice";

            /// <summary>
            /// This is a map of key/value custom parameters which are to be included with the launch.
            /// </summary>
            public const string Custom = "https://purl.imsglobal.org/spec/lti/claim/custom";

            /// <summary>
            /// An opaque value data claim used by Deep Linking.
            /// </summary>
            public const string Data = "https://purl.imsglobal.org/spec/lti-dl/claim/data";

            /// <summary>
            /// Deep Linking settings.
            /// </summary>
            public const string DeepLinkingSettings = "https://purl.imsglobal.org/spec/lti-dl/claim/deep_linking_settings";

            /// <summary>
            /// The deployment identifier, uniquely identifying the tool's deployment on the platform.
            /// </summary>
            public const string DeploymentId = "https://purl.imsglobal.org/spec/lti/claim/deployment_id";

            /// <summary>
            /// Set by the proctoring tool to ask the platform to send an LtiEndAssessment message
            /// when the assessment ends (Proctoring Services 1.0 §4.3).
            /// </summary>
            public const string EndAssessmentReturn = "https://purl.imsglobal.org/spec/lti-ap/claim/end_assessment_return";

            /// <summary>
            /// Optional plain text message.
            /// </summary>
            public const string ErrorLog = "https://purl.imsglobal.org/spec/lti-dl/claim/errorlog";

            /// <summary>
            /// Optional plain text message.
            /// </summary>
            public const string ErrorMessage = "https://purl.imsglobal.org/spec/lti-dl/claim/errormsg";

            /// <summary>
            /// The user whose submission is being reviewed (Submission Review 1.0 §3.2).
            /// </summary>
            public const string ForUser = "https://purl.imsglobal.org/spec/lti/claim/for_user";

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
            /// Optional plain text message.
            /// </summary>
            public const string Log = "https://purl.imsglobal.org/spec/lti-dl/claim/log";

            /// <summary>
            /// Obsolete: LTI 1.1 migration mapping.
            /// </summary>
            public const string Lti11LegacyUserId = "https://purl.imsglobal.org/spec/lti/claim/lti11_legacy_user_id";

            /// <summary>
            /// Optional LTI 1.1 migration mapping.
            /// </summary>
            public const string LtiMigration = "https://purl.imsglobal.org/spec/lti/claim/lti1p1";

            /// <summary>The LTI Platform Configuration claim on an OpenID configuration document (Dynamic Registration §3.5).</summary>
            public const string LtiPlatformConfiguration = "https://purl.imsglobal.org/spec/lti-platform-configuration";

            /// <summary>The LTI Tool Configuration claim on a registration request body (Dynamic Registration §3.6).</summary>
            public const string LtiToolConfiguration = "https://purl.imsglobal.org/spec/lti-tool-configuration";

            /// <summary>
            /// Optional plain text message.
            /// </summary>
            public const string Message = "https://purl.imsglobal.org/spec/lti-dl/claim/msg";

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
            /// Proctoring-specific log message, reported by the proctoring tool on LtiEndAssessment
            /// (Proctoring Services 1.0 §4.4). Distinct from the Deep Linking <see cref="ErrorLog"/> claim.
            /// </summary>
            public const string ProctoringErrorLog = "https://purl.imsglobal.org/spec/lti-ap/claim/errorlog";

            /// <summary>
            /// Proctoring-specific error message intended for the end user, reported by the proctoring tool
            /// on LtiEndAssessment (Proctoring Services 1.0 §4.4). Distinct from the Deep Linking
            /// <see cref="ErrorMessage"/> claim.
            /// </summary>
            public const string ProctoringErrorMessage = "https://purl.imsglobal.org/spec/lti-ap/claim/errormsg";

            /// <summary>
            /// Opaque proctoring configuration passed between the platform and the proctoring tool
            /// (Proctoring Services 1.0 §4.2).
            /// </summary>
            public const string ProctoringSettings = "https://purl.imsglobal.org/spec/lti-ap/claim/proctoring_settings";

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
            /// Opaque value the platform issues on LtiStartProctoring and requires back on
            /// LtiStartAssessment to correlate the two messages (Proctoring Services 1.0 §4.2).
            /// </summary>
            public const string SessionData = "https://purl.imsglobal.org/spec/lti-ap/claim/session_data";

            /// <summary>
            /// The platform URL the proctoring tool POSTs the LtiStartAssessment message to
            /// (Proctoring Services 1.0 §4.2).
            /// </summary>
            public const string StartAssessmentUrl = "https://purl.imsglobal.org/spec/lti-ap/claim/start_assessment_url";

            /// <summary>
            /// The launch url.
            /// </summary>
            public const string TargetLinkUri = "https://purl.imsglobal.org/spec/lti/claim/target_link_uri";

            /// <summary>
            /// The user identity attributes the proctoring tool verified before the assessment started
            /// (Proctoring Services 1.0 §4.3).
            /// </summary>
            public const string VerifiedUser = "https://purl.imsglobal.org/spec/lti-ap/claim/verified_user";

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
            /// Assignment and Grade Services scopes
            /// </summary>
            public static class Ags
            {
                /// <summary>
                /// Assignment and Grade Service lineitem read/write scope.
                /// </summary>
                public const string LineItem = "https://purl.imsglobal.org/spec/lti-ags/scope/lineitem";

                /// <summary>
                /// Assignment and Grade Service lineitem readonly scope.
                /// </summary>
                public const string LineItemReadonly = "https://purl.imsglobal.org/spec/lti-ags/scope/lineitem.readonly";

                /// <summary>
                /// Assignment and Grade Service result readonly scope.
                /// </summary>
                public const string ResultReadonly = "https://purl.imsglobal.org/spec/lti-ags/scope/result.readonly";

                /// <summary>
                /// Assignment and Grade Service score read/write scope.
                /// </summary>
                public const string Score = "https://purl.imsglobal.org/spec/lti-ags/scope/score";

                /// <summary>
                /// Custom Assignment and Grade Service score readonly scope.
                /// </summary>
                public const string ScoreReadonly = "https://purl.imsglobal.org/spec/lti-ags/scope/score.readonly";
            }

            /// <summary>
            /// Proctoring Services scopes.
            /// </summary>
            public static class Ap
            {
                /// <summary>
                /// Assessment Control Service scope, required to invoke any control action
                /// (Proctoring Services 1.0 §5.1).
                /// </summary>
                public const string ControlAll = "https://purl.imsglobal.org/spec/lti-ap/scope/control.all";
            }

            /// <summary>LTI Dynamic Registration scopes (Dynamic Registration §4.4).</summary>
            public static class DynamicRegistration
            {
                /// <summary>Scope required on the registration access token.</summary>
                public const string Scope = "https://purl.imsglobal.org/spec/lti-reg/scope/registration";
            }

            /// <summary>
            /// Course Groups Service scopes.
            /// </summary>
            public static class Gs
            {
                /// <summary>
                /// Course Groups Service readonly scope, required for both the groups and the
                /// group sets endpoints (Course Groups Service 1.0 §3).
                /// </summary>
                public const string ContextGroupReadonly = "https://purl.imsglobal.org/spec/lti-gs/scope/contextgroup.readonly";
            }

            /// <summary>
            /// Names and Role Provisioning Service scopes.
            /// </summary>
            public static class Nrps
            {
                /// <summary>
                /// Names and Role Service readonly scope.
                /// </summary>
                public const string MembershipReadonly = "https://purl.imsglobal.org/spec/lti-nrps/scope/contextmembership.readonly";
            }
        }

        /// <summary>
        /// Service media types.
        /// </summary>
        public static class MediaTypes
        {
            /// <summary>
            /// https://www.imsglobal.org/spec/lti-gs/v1p0#context-group-container-media-type
            /// </summary>
            public const string ContextGroupContainer = "application/vnd.ims.lti-gs.v1.contextgroupcontainer+json";

            /// <summary>
            /// https://www.imsglobal.org/spec/lti-gs/v1p0#context-group-set-container-media-type
            /// </summary>
            public const string ContextGroupSetContainer = "application/vnd.ims.lti-gs.v1.contextgroupsetcontainer+json";

            /// <summary>
            /// JSON Web Key Set media type (RFC 7517 §6).
            /// </summary>
            public const string Jwks = "application/jwk-set+json";

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
            public const string MembershipContainer = "application/vnd.ims.lti-nrps.v2.membershipcontainer+json";

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
            /// Assignment and Grade Services endpoints.
            /// </summary>
            public static class Ags
            {
                /// <summary>
                /// Line item endpoint.
                /// </summary>
                public const string LineItemService = "lineitem";

                /// <summary>
                /// Line items endpoint.
                /// </summary>
                public const string LineItemsService = "lineitems";

                /// <summary>
                /// Results endpoint.
                /// </summary>
                public const string ResultsService = "results";

                /// <summary>
                /// Score endpoint.
                /// </summary>
                public const string ScoreService = "score";

                /// <summary>
                /// Scores endpoint.
                /// </summary>
                public const string ScoresService = "scores";
            }

            /// <summary>
            /// Proctoring Services endpoints.
            /// </summary>
            public static class Ap
            {
                /// <summary>
                /// Assessment Control Service endpoint.
                /// </summary>
                public const string AssessmentControlService = "assessment-control";
            }

            /// <summary>
            /// Course Groups Service endpoints.
            /// </summary>
            public static class Gs
            {
                /// <summary>
                /// Context group sets endpoint.
                /// </summary>
                public const string GroupSetsService = "group-sets";

                /// <summary>
                /// Context groups endpoint.
                /// </summary>
                public const string GroupsService = "groups";
            }

            /// <summary>
            /// JWKS publication endpoint.
            /// </summary>
            public static class Jwks
            {
                /// <summary>The well-known JWKS endpoint route name.</summary>
                public const string JwksService = "jwks";
            }

            /// <summary>
            /// Names and Role Provisioning Service endpoints.
            /// </summary>
            public static class Nrps
            {
                /// <summary>
                /// Membership endpoint.
                /// </summary>
                public const string MembershipService = "membership";
            }
        }
    }
}
