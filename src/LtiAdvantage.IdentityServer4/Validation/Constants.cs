namespace LtiAdvantage.IdentityServer4.Validation
{
    /// <summary>
    /// Identity Server constants used by <see cref="PrivatePemKeyJwtSecretValidator"/>.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Types of ClientSecret used by <see cref="PrivatePemKeyJwtSecretValidator"/>.
        /// </summary>
        public static class SecretTypes
        {
            /// <summary>
            /// The ClientSecret is a private key in PEM format.
            /// </summary>
            public const string PrivatePemKey = "PrivatePemKey";

            /// <summary>
            /// The ClientSecret is a public key in PEM format.
            /// </summary>
            public const string PublicPemKey = "PublicPemKey";
        }
    }
}
