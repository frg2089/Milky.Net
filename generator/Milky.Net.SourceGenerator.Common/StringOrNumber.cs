using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.SourceGenerator.Common;

[JsonConverter(typeof(StringOrNumberJsonConverter))]
public sealed class StringOrNumber
{
    public StringOrNumber()
    {
    }

    public StringOrNumber(string value)
        => String = value;

    public StringOrNumber(int value)
        => Number = value;

    public string? String { get; set; }
    public int? Number { get; set; }

    private class StringOrNumberJsonConverter : JsonConverter<StringOrNumber>
    {
        public override StringOrNumber? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => new(reader.GetString()!),
                JsonTokenType.Number => new(reader.GetInt32()),
                _ => throw new NotSupportedException(reader.TokenType.ToString()),
            };
        }

        public override void Write(Utf8JsonWriter writer, StringOrNumber value, JsonSerializerOptions options)
        {
            if (value.Number.HasValue)
                writer.WriteNumberValue(value.Number.Value);
            else
                writer.WriteStringValue(value.String);
        }
    }
}