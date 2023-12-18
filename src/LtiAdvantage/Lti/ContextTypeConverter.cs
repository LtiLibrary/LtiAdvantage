using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using LtiAdvantage.Utilities;

namespace LtiAdvantage.Lti
{
    internal class ContextTypeConverter : JsonConverter<ContextType>
    {
        private static readonly Dictionary<string, ContextType> UriToContextTypeMap = new();

        static ContextTypeConverter()
        {
            // Building the mapping between the URI strings and ContextType enum values
            foreach (var field in typeof(ContextType).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attrs = field.GetCustomAttributes<UriAttribute>();
                foreach (var attr in attrs)
                {
                    UriToContextTypeMap[attr.Uri] = (ContextType)field.GetValue(null);
                }
            }
        }

        public override ContextType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return ContextType.Unknown; // Assuming default is equivalent to ContextType.Unknown
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Expected a string value for the enum deserialization.");
            }

            var value = reader.GetString();
            if (value != null && UriToContextTypeMap.TryGetValue(value, out var contextType))
            {
                return contextType;
            }

            return ContextType.Unknown; // Assuming Unknown is a defined enum value for invalid/missing cases
        }

        public override void Write(Utf8JsonWriter writer, ContextType value, JsonSerializerOptions options)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            var member = type.GetMember(name)[0];
            var attrs = member.GetCustomAttributes<UriAttribute>();

            if (attrs.Any())
            {
                var uri = attrs.First().Uri; // Assuming you want to use the first Uri if there are multiple
                writer.WriteStringValue(uri);
                return;
            }

            writer.WriteStringValue(name); // Fallback to the enum name if no Uri is found
        }
    }
}
