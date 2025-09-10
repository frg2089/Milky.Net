using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ObjectTypeInfoData), "object")]
[JsonDerivedType(typeof(UnionTypeInfoData), "union")]
[JsonDerivedType(typeof(EnumTypeInfoData), "enum")]
public abstract record TypeInfoData(
    [property: JsonPropertyName("description")] string? Description
);

public sealed record ObjectTypeInfoData(
    string? Description,
    [property: JsonPropertyName("properties")] Dictionary<string, PropertyInfoData> Properties,
    [property: JsonPropertyName("baseType")] string? BaseType
) : TypeInfoData(Description);

public sealed record UnionTypeInfoData(
    string? Description,
    [property: JsonPropertyName("types")] Dictionary<string, string> Types,
    [property: JsonPropertyName("discriminator")] string Discriminator
) : TypeInfoData(Description);

public sealed record EnumTypeInfoData(
    string? Description,
    [property: JsonPropertyName("items")] Dictionary<string, StringOrNumber> Items
) : TypeInfoData(Description);

public sealed record PropertyInfoData(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("constants")] StringOrNumber[] Constants
);

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