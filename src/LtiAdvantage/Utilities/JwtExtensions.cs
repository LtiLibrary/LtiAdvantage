﻿using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LtiAdvantage.Utilities
{
    /// <summary>
    /// Extensions to make working with JWT Tokens easier.
    /// </summary>
    internal static class JwtExtensions
    {
        private static readonly JsonSerializerOptions Settings = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
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
                return typeof(T) == typeof(string)
                    ? JsonSerializer.Deserialize<T>($"\"{value}\"")
                    : JsonSerializer.Deserialize<T>(value.ToString());
            }

            return default(T);
        }

        private static T GetClaimValues<T>(this JwtPayload payload, string type)
        {
            var values = payload.Claims
                .Where(c => c.Type == type)
                .Select(c => c.Value).ToArray();

            if (0 == values.Length)
                return default(T);

            var elementType = typeof(T).GetElementType();
            if (elementType != null && elementType.IsClass && !elementType.IsEquivalentTo(typeof(string)))
            {
                return JsonSerializer.Deserialize<T>("[" + string.Join(",", values) + "]");
            }
            return JsonSerializer.Deserialize<T>("[\"" + string.Join("\",\"", values) + "\"]");
        }

        public static void SetClaimValue<T>(this JwtPayload payload, string type, T value)
        {
            if (payload.ContainsKey(type))
            {
                payload.Remove(type);
            }

            if (typeof(T) == typeof(string))
            {
                var stringValue = value?.ToString();
                if (!string.IsNullOrWhiteSpace(stringValue))
                {
                    payload.AddClaim(new Claim(type, stringValue, ClaimValueTypes.String));
                }
            } 
            else if (typeof(T) == typeof(int))
            {
                payload.AddClaim(new Claim(type, value.ToString(), ClaimValueTypes.Integer));
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
