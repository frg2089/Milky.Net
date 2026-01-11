using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;
public sealed record class Api(
    [property: JsonPropertyName("endpoint")] string Endpoint,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("responseFields")] IReadOnlyList<Field>? ResponseFields,
    [property: JsonPropertyName("requestFields")] IReadOnlyList<Field>? RequestFields
) : IMilkyType;
