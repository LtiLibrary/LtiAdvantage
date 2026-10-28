using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LtiAdvantage.Proctoring
{
    /// <summary>
    /// Reads an enum from a case-insensitive string and writes it in lowercase. Null, empty and
    /// unrecognized values read as <c>default(TEnum)</c>, which must be the enum's "unknown"
    /// member (value 0); that member is written as JSON null.
    /// </summary>
    internal class LowercaseEnumConverter<TEnum> : JsonConverter<TEnum> where TEnum : struct, Enum
    {
        public override TEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var value = reader.GetString();

            // Only an exact, defined member name; numbers and comma lists read as unknown.
            return EnumNames.TryParse<TEnum>(value, out var parsed) ? parsed : default;
        }

        public override void Write(Utf8JsonWriter writer, TEnum value, JsonSerializerOptions options)
        {
            if (value.Equals(default(TEnum)))
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
