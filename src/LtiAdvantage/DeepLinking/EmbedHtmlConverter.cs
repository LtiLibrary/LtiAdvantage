using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LtiAdvantage.DeepLinking
{
    /// <summary>
    /// Reads and writes a content item's <c>embed</c> property, an object with an <c>html</c>
    /// member, as the HTML string it carries.
    /// </summary>
    internal class EmbedHtmlConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.String:
                    // 4.0.0 wrote the HTML as a plain string.
                    return reader.GetString();
                case JsonTokenType.StartObject:
                    using (var document = JsonDocument.ParseValue(ref reader))
                    {
                        foreach (var property in document.RootElement.EnumerateObject())
                        {
                            if (string.Equals(property.Name, "html", StringComparison.OrdinalIgnoreCase))
                            {
                                return property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() : null;
                            }
                        }

                        return null;
                    }
                default:
                    throw new JsonException("Expected an embed object.");
            }
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("html", value);
            writer.WriteEndObject();
        }
    }
}
