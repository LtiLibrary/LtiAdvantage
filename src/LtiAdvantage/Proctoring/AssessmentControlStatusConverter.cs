using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LtiAdvantage.Proctoring
{
    internal class AssessmentControlStatusConverter : JsonConverter<AssessmentControlStatus>
    {
        public override AssessmentControlStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return AssessmentControlStatus.Unknown;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var value = reader.GetString();

            if (string.IsNullOrEmpty(value))
            {
                return AssessmentControlStatus.Unknown;
            }

            return Enum.TryParse<AssessmentControlStatus>(value, true, out var status)
                ? status
                : AssessmentControlStatus.Unknown;
        }

        public override void Write(Utf8JsonWriter writer, AssessmentControlStatus value, JsonSerializerOptions options)
        {
            if (value == AssessmentControlStatus.Unknown)
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
