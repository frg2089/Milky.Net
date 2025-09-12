using System.Text.Json.Serialization;

namespace Milky.Net.SourceGenerator.Common;

public sealed record GenericTypeInfoData(
    string? Description,
    [property: JsonPropertyName("baseType")] string BaseType,
    [property: JsonPropertyName("genericPropertyName")] string GenericPropertyName
) : TypeInfoData(Description);

