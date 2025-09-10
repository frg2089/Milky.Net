using System.Text.Json.Serialization;

namespace Milky.Net.SourceGenerator.Common;

public sealed record ObjectTypeInfoData(
    string? Description,
    [property: JsonPropertyName("properties")] Dictionary<string, PropertyInfoData> Properties,
    [property: JsonPropertyName("baseType")] string? BaseType
) : TypeInfoData(Description);
