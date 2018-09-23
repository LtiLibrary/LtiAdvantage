namespace LtiAdvantageLibrary.NetCore.Lti
{
    /// <summary>
    /// Subset of OpenID standard claims used by LTI Advantage. See https://openid.net/specs/openid-connect-core-1_0.html#Claims.
    /// </summary>
    public static class OpenIdStandardClaimNames
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
}
