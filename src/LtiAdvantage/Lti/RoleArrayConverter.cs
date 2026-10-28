using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LtiAdvantage.Lti
{
    /// <summary>
    /// Reads and writes a roles array as role URIs.
    /// </summary>
    /// <remarks>
    /// <see cref="RoleConverter"/> writes <see cref="Role.Unknown"/> as null, which inside a roles
    /// array gives <c>["…#Learner", null]</c>, not the array of URI strings the specs require.
    /// This converter leaves unknown roles out instead. When reading, a role the library does not
    /// define reads as <see cref="Role.Unknown"/>, and null elements are skipped.
    /// </remarks>
    internal class RoleArrayConverter : JsonConverter<Role[]>
    {
        public override Role[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected an array of roles.");
            }

            var roles = new List<Role>();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.Null:
                        continue;
                    case JsonTokenType.String:
                        roles.Add(RoleExtensions.TryParse(reader.GetString(), out var role) ? role : Role.Unknown);
                        break;
                    default:
                        throw new JsonException("Expected each role to be a string.");
                }
            }

            return roles.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, Role[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var role in value)
            {
                if (role == Role.Unknown)
                {
                    continue;
                }

                writer.WriteStringValue(role.ToUri() ?? role.ToString());
            }

            writer.WriteEndArray();
        }
    }
}
