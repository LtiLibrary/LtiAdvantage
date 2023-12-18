using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LtiAdvantage.Utilities
{
    public class IsoDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Iso8601Format = "yyyy-MM-ddTHH:mm:ss.fffZ";

        private readonly string[] _dateFormats = new string[]
        {
        "yyyy-MM-ddTHH:mm:ss.fffZ",  // Assume UTC if no timezone is provided
        "yyyy-MM-ddTHH:mm:ss.fffK",  // DateTime with DateTimeKind specified
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ssK",
        "yyyy-MM-ddTHH:mm:ss.fffffffK",
            // Add more formats to handle here if needed
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateString = reader.GetString();
            if (dateString != null)
            {
                // Try to parse the date string using the predefined formats
                if (DateTime.TryParseExact(dateString, _dateFormats, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime date))
                {
                    return date;
                }

                // If none of the predefined formats work, fall back to more general parsing
                if (DateTime.TryParse(dateString, out date))
                {
                    return date;
                }
            }

            // If parsing fails, you could either throw an exception, return DateTime.MinValue, or handle it in another appropriate way
            throw new JsonException("Unable to parse date time value.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString(Iso8601Format, CultureInfo.InvariantCulture));
        }
    }

}
