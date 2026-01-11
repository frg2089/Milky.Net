using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

public sealed record class MilkyIR(
    [property: JsonPropertyName("milkyVersion")] string MilkyVersion,
    [property: JsonPropertyName("milkyPackageVersion")] string MilkyPackageVersion,
    [property: JsonPropertyName("commonStructs")] IReadOnlyList<MilkyType> CommonStructs,
    [property: JsonPropertyName("apiCategories")] IReadOnlyList<ApiCategory> ApiCategories)
{
    public static MilkyIR ParseFromJson(JsonElement json)
    {
        MilkyIRParser parser = new(json);
        return new(parser.MilkyVersion, parser.MilkyPackageVersion, parser.CommonStructs, parser.ApiCategories);
    }
}

file sealed class MilkyIRParser(JsonElement root)
{
    public string MilkyVersion => root.GetStringValue("milkyVersion");
    public string MilkyPackageVersion => root.GetStringValue("milkyPackageVersion");
    public IReadOnlyList<MilkyType> CommonStructs => [.. ParseTypeCollection()];
    public IReadOnlyList<ApiCategory> ApiCategories => root.GetProperty<IReadOnlyList<ApiCategory>>("apiCategories");

    private IEnumerable<MilkyType> ParseTypeCollection()
    {
        using var enumerator = root.GetProperty("commonStructs").EnumerateArray();
        foreach (var item in enumerator)
            yield return ParseType(item);
    }

    private static MilkyType ParseType(JsonElement json)
        => json.GetStringValue("structType") switch
        {
            "simple" => json.Deserialize<SimpleMilkyType>() as MilkyType,
            "union" => ParseUnionType(json),
            string structType => throw new InvalidCastException($"Unknown structType {structType}."),
            _ => throw new InvalidCastException("Cannot found structType."),
        } ?? throw new InvalidCastException();

    private static UnionMilkyType ParseUnionType(JsonElement json)
        => json.GetStringValue("unionType") switch
        {
            "plain" => json.Deserialize<SimpleUnionType>() as UnionMilkyType,
            "withData" => ParseAdvancedUnionType(json),
            string unionType => throw new InvalidCastException($"Unknown unionType {unionType}."),
            _ => throw new InvalidCastException("Cannot found unionType."),
        } ?? throw new InvalidCastException();

    private static AdvancedUnionType ParseAdvancedUnionType(JsonElement json)
    {
        var tmp = ParseDerivedTypeCollection(json.GetProperty("derivedTypes"));
        return new(
            json.GetStringValue("name"),
            json.GetStringValue("description"),
            json.GetStringValue("tagFieldName"),
            json.GetProperty<IReadOnlyList<Field>>("baseFields"),
            [.. tmp]);
    }

    private static IEnumerable<MilkyDerivedType> ParseDerivedTypeCollection(JsonElement json)
    {
        using var enumerator = json.EnumerateArray();
        foreach (var item in enumerator)
            yield return ParseDerivedType(item);
    }

    private static MilkyDerivedType ParseDerivedType(JsonElement json)
        => json.GetStringValue("derivingType") switch
        {
            "struct" => json.Deserialize<SimpleDerivedType>() as MilkyDerivedType,
            "ref" => json.Deserialize<RefDerivedType>(),
            string derivingType => throw new InvalidCastException($"Unknown derivingType {derivingType}."),
            _ => throw new InvalidCastException("Cannot found derivingType."),
        } ?? throw new InvalidCastException();
}

file static class JsonElementExtensions
{
    extension(JsonElement json)
    {
        public string GetStringValue(string property)
            => json.GetProperty(property).GetString()
            ?? throw new JsonException($"Cannot found property {property}");

        public T GetProperty<T>(string property)
            => json.GetProperty(property).Deserialize<T>()
            ?? throw new JsonException($"Cannot found property {property}");
    }
}
