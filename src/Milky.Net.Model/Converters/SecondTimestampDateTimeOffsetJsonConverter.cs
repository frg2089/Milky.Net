using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.Model;

public sealed class SecondTimestampDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        reader.Read();
        var i = reader.GetInt64();
        return DateTimeOffset.FromUnixTimeSeconds(i);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}
