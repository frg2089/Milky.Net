using System.Text.Json.Serialization;

namespace Milky.Net.SourceGenerator.Common;

public sealed record UnionTypeInfoData(
    string? Description,
    [property: JsonPropertyName("types")] Dictionary<string, string> Types,
    [property: JsonPropertyName("discriminator")] string Discriminator
) : TypeInfoData(Description);
