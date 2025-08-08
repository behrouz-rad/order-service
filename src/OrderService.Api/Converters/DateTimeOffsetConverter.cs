// © 2025 Behrouz Rad. All rights reserved.

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OrderService.Api.Converters;

public sealed class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTimeOffset.Parse(reader.GetString()!, CultureInfo.InvariantCulture);
    }

    public override void Write(
        Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        var utc = value.ToUniversalTime();
        writer.WriteStringValue(utc.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
    }
}
