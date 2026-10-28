using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("LtiAdvantage.UnitTests")]
namespace LtiAdvantage.Utilities
{
    /// <summary>
    /// Extensions to make working with JWT Tokens easier.
    /// </summary>
    internal static class JwtExtensions
    {
        // Platforms are not consistent about quoting numbers (the Proctoring spec's own examples
        // send attempt_number both as 1 and as "1"), so claim values accept both when read.
        // Names are matched ignoring case so content items written by 4.0.0, which used
        // "Width", "Height" and "ExpiresAt", still read.
        private static readonly JsonSerializerOptions ReadSettings = new JsonSerializerOptions
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNameCaseInsensitive = true
        };

        // Role arrays (the roles claim) leave unknown roles out rather than writing nulls.
        private static readonly JsonSerializerOptions Settings = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = { new Lti.RoleArrayConverter() }
        };

        /// <summary>
        /// Get the payload claim value as a string.
        /// </summary>
        /// <returns>The claim value as a string.</returns>
        public static string GetClaimValue(this JwtPayload payload, string type)
        {
            return GetClaimValue<string>(payload, type);
        }

        /// <summary>
        /// Get the payload claim value as an object of type T.
        /// </summary>
        /// <typeparam name="T">The expected Type of the claim value.</typeparam>
        /// <param name="payload">The <see cref="JwtPayload"/> with the claim.</param>
        /// <param name="type">The claim type.</param>
        /// <returns>The claim value as an object of type T.</returns>
        public static T GetClaimValue<T>(this JwtPayload payload, string type)
        {
            if (typeof(T).IsArray)
            {
                return GetClaimValues<T>(payload, type);
            }

            if (payload.TryGetValue(type, out var value))
            {
                return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value), ReadSettings);
            }

            return default(T);
        }

        private static T GetClaimValues<T>(this JwtPayload payload, string type)
        {
            var claims = payload.Claims
                .Where(c => c.Type == type)
                .ToArray();

            if (0 == claims.Length)
                return default(T);

            // Each array element becomes its own claim. Object elements (e.g. deep linking
            // content items) carry their JSON as the claim value; keep them as objects
            // rather than JSON strings so they deserialize into the element type.
            var values = new JsonArray(claims
                .Select(c => c.ValueType is JsonClaimValueTypes.Json or JsonClaimValueTypes.JsonArray
                    ? JsonNode.Parse(c.Value)
                    : JsonValue.Create(c.Value))
                .ToArray());

            // ReadSettings: numbers written as strings are accepted here too.
            return values.Deserialize<T>(ReadSettings);
        }

        private static Type ScalarType<T>() => Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        public static void SetClaimValue<T>(this JwtPayload payload, string type, T value)
        {
            if (payload.ContainsKey(type))
            {
                payload.Remove(type);
            }

            // Setting a claim to null removes it; an absent claim and a "null" one are not the same
            // to a strict receiver.
            if (value == null)
            {
                return;
            }

            if (typeof(T) == typeof(string))
            {
                var stringValue = value?.ToString();
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    payload.AddClaim(new Claim(type, stringValue, ClaimValueTypes.String));
                }
            } 
            else if (ScalarType<T>() == typeof(int) || ScalarType<T>() == typeof(long))
            {
                // int? and long? unwrap to the same typed claim as int, not a JSON claim.
                payload.AddClaim(new Claim(type, Convert.ToString(value, CultureInfo.InvariantCulture),
                    ScalarType<T>() == typeof(int) ? ClaimValueTypes.Integer : ClaimValueTypes.Integer64));
            }
            else if (ScalarType<T>() == typeof(bool))
            {
                payload.AddClaim(new Claim(type, (bool) (object) value ? "true" : "false", ClaimValueTypes.Boolean));
            }
            else if (typeof(T).IsArray)
            {
                payload.AddClaim(new Claim(type, JsonSerializer.Serialize(value, Settings), JsonClaimValueTypes.JsonArray));
            }
            else
            {
                var json = JsonSerializer.Serialize(value, Settings);
                payload.AddClaim(new Claim(type, json, JsonClaimValueTypes.Json));
            }
        }
    }
}
