using System.Text.Json.Serialization;

namespace Milky.Net.SourceGenerator.Common;

public sealed record ApiEndPoint(
    [property: JsonPropertyName("endpoint")] string Endpoint,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("inputStruct")] string? InputStruct,
    [property: JsonPropertyName("outputStruct")] string? OutputStruct
);
