using System.Text.Json.Serialization;

namespace Milky.Net.SourceGenerator.Common;

public sealed record PropertyInfoData(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("constants")] StringOrNumber[] Constants
);