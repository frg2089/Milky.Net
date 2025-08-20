using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Client.SourceGenerator.Data;

public sealed record ApiEndPoints(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("apis")] IReadOnlyList<ApiEndPoint> Apis
);
