namespace LtiAdvantage.IdentityServer4
{
    /// <summary>
    /// Identity Server contants used by <see cref="PublicKeyJwtSecretValidator"/>.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Types of ClientSecret used by <see cref="PublicKeyJwtSecretValidator"/>.
        /// </summary>
        public static class SecretTypes
        {
            /// <summary>
            /// The ClientSecret is a public key in PEM format.
            /// </summary>
            public const string PublicPemKey = "PublicPemKey";

            /// <summary>
            /// The ClientSecret is a public key in JSON format.
            /// </summary>
            public const string PublicJsonWebKey = "PublicJsonWebKey";
        }
    }
}
