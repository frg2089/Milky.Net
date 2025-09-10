using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.Model;

public sealed class MilkyUriJsonConverter : JsonConverter<MilkyUri>
{
    public override MilkyUri? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetString() switch
    {
        string str => new(str),
        _ => null
    };

    public override void Write(Utf8JsonWriter writer, MilkyUri value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.OriginalUri);
}