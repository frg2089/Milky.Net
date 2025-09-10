using System.Text.Json.Serialization;

namespace Milky.Net.SourceGenerator.Common;

public sealed record ApiEndPoints(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("apis")] IReadOnlyList<ApiEndPoint> Apis
);
