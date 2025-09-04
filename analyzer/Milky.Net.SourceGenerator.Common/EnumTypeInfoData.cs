using System.Text.Json.Serialization;

namespace Milky.Net.SourceGenerator.Common;

public sealed record EnumTypeInfoData(
    string? Description,
    [property: JsonPropertyName("items")] Dictionary<string, StringOrNumber> Items
) : TypeInfoData(Description);