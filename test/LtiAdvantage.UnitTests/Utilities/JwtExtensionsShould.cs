using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LtiAdvantage.Utilities;
using System.Text.Json;
using Xunit;

namespace LtiAdvantage.UnitTests.Utilities
{
    public class JwtExtensionsShould
    {
        /// <summary>
        /// Verifies that GetClaimValue can retrieve a string claim value.
        /// </summary>
        [Fact]
        public void GetClaimValue_ReturnStringValue()
        {
            var payload = new JwtPayload();
            var claimType = "name";
            var claimValue = "John Doe";
            payload.AddClaim(new Claim(claimType, claimValue));

            var result = payload.GetClaimValue(claimType);

            Assert.Equal(claimValue, result);
        }

        /// <summary>
        /// Verifies that GetClaimValue can handle double quotes in string claim values.
        /// </summary>
        [Fact]
        public void GetClaimValue_HandleDoubleQuotesInString()
        {
            var payload = new JwtPayload();
            var claimType = "quote";
            var claimValue = "He said, \"Hello, World!\"";
            payload.AddClaim(new Claim(claimType, claimValue));

            var result = payload.GetClaimValue(claimType);

            Assert.Equal(claimValue, result);
        }

        /// <summary>
        /// Verifies that GetClaimValue can retrieve an integer claim value.
        /// </summary>
        [Fact]
        public void GetClaimValue_ReturnIntegerValue()
        {
            var payload = new JwtPayload();
            var claimType = "age";
            var claimValue = 30;
            payload.AddClaim(new Claim(claimType, claimValue.ToString(), ClaimValueTypes.Integer));

            var result = payload.GetClaimValue<int>(claimType);

            Assert.Equal(claimValue, result);
        }

        /// <summary>
        /// Verifies that GetClaimValue returns default when claim is not found.
        /// </summary>
        [Fact]
        public void GetClaimValue_ReturnDefaultWhenClaimNotFound()
        {
            var payload = new JwtPayload();
            var claimType = "nonexistent";

            var result = payload.GetClaimValue<string>(claimType);

            Assert.Null(result);
        }

        /// <summary>
        /// Verifies that GetClaimValues can retrieve an array of string claim values.
        /// </summary>
        [Fact]
        public void GetClaimValues_ReturnStringArray()
        {
            var payload = new JwtPayload();
            var claimType = "roles";
            var claimValues = new[] { "admin", "user", "editor" };
            foreach (var value in claimValues)
            {
                payload.AddClaim(new Claim(claimType, value));
            }

            var result = payload.GetClaimValue<string[]>(claimType);

            Assert.Equal(claimValues, result);
        }

        /// <summary>
        /// Verifies that GetClaimValues can handle arrays with special characters.
        /// </summary>
        [Fact]
        public void GetClaimValues_HandleSpecialCharactersInArray()
        {
            var payload = new JwtPayload();
            var claimType = "quotes";
            var claimValues = new[] { "Value with \"quotes\"", "Another \"value\"" };
            foreach (var value in claimValues)
            {
                payload.AddClaim(new Claim(claimType, value));
            }

            var result = payload.GetClaimValue<string[]>(claimType);

            Assert.Equal(claimValues, result);
        }

        /// <summary>
        /// Verifies that SetClaimValue can add a string claim to the payload.
        /// </summary>
        [Fact]
        public void SetClaimValue_AddStringClaim()
        {
            var payload = new JwtPayload();
            var claimType = "nickname";
            var claimValue = "Johnny";

            payload.SetClaimValue(claimType, claimValue);

            var result = payload.Claims.FirstOrDefault(c => c.Type == claimType)?.Value;
            Assert.Equal(claimValue, result);
        }

        /// <summary>
        /// Verifies that SetClaimValue can add an integer claim to the payload.
        /// </summary>
        [Fact]
        public void SetClaimValue_AddIntegerClaim()
        {
            var payload = new JwtPayload();
            var claimType = "score";
            var claimValue = 100;

            payload.SetClaimValue(claimType, claimValue);

            var result = payload.GetClaimValue<int>(claimType);
            Assert.Equal(claimValue, result);
        }

        /// <summary>
        /// Verifies that SetClaimValue can add an array of strings as a claim.
        /// </summary>
        [Fact]
        public void SetClaimValue_AddStringArrayClaim()
        {
            var payload = new JwtPayload();
            var claimType = "permissions";
            var claimValue = new[] { "read", "write", "execute" };

            payload.SetClaimValue(claimType, claimValue);

            var result = payload.GetClaimValue<string[]>(claimType);
            Assert.Equal(claimValue, result);
        }

        /// <summary>
        /// Verifies that SetClaimValue overwrites existing claims of the same type.
        /// </summary>
        [Fact]
        public void SetClaimValue_OverwriteExistingClaim()
        {
            var payload = new JwtPayload();
            var claimType = "email";
            var initialValue = "old.email@example.com";
            var newValue = "new.email@example.com";
            payload.AddClaim(new Claim(claimType, initialValue));

            payload.SetClaimValue(claimType, newValue);

            var result = payload.GetClaimValue(claimType);
            Assert.Equal(newValue, result);
        }

        /// <summary>
        /// Verifies that GetClaimValue can handle JSON-formatted claim values.
        /// </summary>
        [Fact]
        public void GetClaimValue_HandleJsonFormattedValue()
        {
            var payload = new JwtPayload();
            var claimType = "metadata";
            var claimObject = new { Key = "Value", Number = 42 };
            var claimValue = JsonSerializer.Serialize(claimObject);
            payload.AddClaim(new Claim(claimType, claimValue, JsonClaimValueTypes.Json));

            var result = payload.GetClaimValue<JsonElement>(claimType);

            Assert.Equal("Value", result.GetProperty("Key").GetString());
            Assert.Equal(42, result.GetProperty("Number").GetInt32());
        }

        /// <summary>
        /// Verifies that GetClaimValue returns default for empty payload.
        /// </summary>
        [Fact]
        public void GetClaimValue_ReturnDefaultForEmptyPayload()
        {
            var payload = new JwtPayload();
            var claimType = "anyClaim";

            var result = payload.GetClaimValue<string>(claimType);

            Assert.Null(result);
        }

        /// <summary>
        /// Verifies that SetClaimValue removes existing claims before adding new ones.
        /// </summary>
        [Fact]
        public void SetClaimValue_RemoveExistingClaimsBeforeAdding()
        {
            var payload = new JwtPayload();
            var claimType = "role";
            payload.AddClaim(new Claim(claimType, "user"));
            payload.AddClaim(new Claim(claimType, "admin"));

            payload.SetClaimValue(claimType, "superuser");

            var claims = payload.Claims.Where(c => c.Type == claimType).ToList();
            Assert.Single(claims);
            Assert.Equal("superuser", claims.First().Value);
        }
    }
}
