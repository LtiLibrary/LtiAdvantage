using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LtiAdvantage.Proctoring
{
    internal class AssessmentControlActionConverter : JsonConverter<AssessmentControlAction>
    {
        public override AssessmentControlAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return AssessmentControlAction.Unknown;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var value = reader.GetString();

            if (string.IsNullOrEmpty(value))
            {
                return AssessmentControlAction.Unknown;
            }

            return Enum.TryParse<AssessmentControlAction>(value, true, out var action)
                ? action
                : AssessmentControlAction.Unknown;
        }

        public override void Write(Utf8JsonWriter writer, AssessmentControlAction value, JsonSerializerOptions options)
        {
            if (value == AssessmentControlAction.Unknown)
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
