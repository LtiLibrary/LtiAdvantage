using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    internal class RoleConverter : JsonConverter<Role>
    {
        private static readonly Hashtable Roles = GetUris(typeof(Role));

        public override Role Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default; // or Role.Unknown depending on how you want to handle nulls
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Expected a string value.");
            }

            string value = reader.GetString();
            if (value != null && Roles.ContainsKey(value))
            {
                return (Role)Roles[value];
            }

            return Role.Unknown; // or another default value for unknown role strings
        }

        public override void Write(Utf8JsonWriter writer, Role value, JsonSerializerOptions options)
        {
            if (value == Role.Unknown)
            {
                writer.WriteNullValue();
                return;
            }

            if (Roles.ContainsKey(value))
            {
                string uri = (string)Roles[value];
                writer.WriteStringValue(uri);
            }
            else
            {
                // If there's no URI representation, write the enum as string
                writer.WriteStringValue(value.ToString());
            }
        }

        private static Hashtable GetUris(Type type)
        {
            var roles = new Hashtable();
            foreach (Enum value in Enum.GetValues(type))
            {
                var uriAttributes = value.GetType().GetField(value.ToString())
                                        .GetCustomAttributes<UriAttribute>()
                                        .ToArray();

                if (uriAttributes.Length > 0)
                {
                    foreach (var uriAttribute in uriAttributes)
                    {
                        roles.Add(uriAttribute.Uri, value);
                        // Only map the Enum back to the full URI
                        if (uriAttribute.Uri.StartsWith("http"))
                        {
                            roles.Add(value, uriAttribute.Uri);
                        }
                    }
                }
            }

            return roles;
        }
    }
}
