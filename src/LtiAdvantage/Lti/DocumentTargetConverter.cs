using System;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace LtiAdvantage.Lti
{
    internal class DocumentTargetConverter : JsonConverter<DocumentTarget>
    {
        public override DocumentTarget Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return DocumentTarget.None;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var value = reader.GetString();

            if (string.IsNullOrEmpty(value))
            {
                return DocumentTarget.None;
            }

            return Enum.TryParse<DocumentTarget>(value, true, out var target)
                ? target
                : DocumentTarget.None;
        }

        public override void Write(Utf8JsonWriter writer, DocumentTarget value, JsonSerializerOptions options)
        {
            if (value == DocumentTarget.None)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStringValue(value.ToString().ToLowerInvariant());
            }
        }
    }
}
