using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

public sealed record class ApiCategory(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("apis")] IReadOnlyList<Api> Apis
);
