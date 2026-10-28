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
                    return ReadEmbedObject(ref reader);
                default:
                    throw new JsonException("Expected embed to be an object with an html member.");
            }
        }

        private static string ReadEmbedObject(ref Utf8JsonReader reader)
        {
            string html = null;
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                var isHtml = reader.ValueTextEquals("html");
                reader.Read();
                if (!isHtml)
                {
                    reader.Skip();
                    continue;
                }

                html = reader.TokenType switch
                {
                    JsonTokenType.String => reader.GetString(),
                    JsonTokenType.Null => null,
                    _ => throw new JsonException("Expected embed.html to be a string.")
                };
            }

            return html;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("html", value);
            writer.WriteEndObject();
        }
    }
}
