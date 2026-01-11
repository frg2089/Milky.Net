using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

public sealed record class EnumField(
    string Name,
    string Description,
    bool? IsArray,
    bool? IsOptional,
    JsonElement? DefaultValue,
    [property: JsonPropertyName("values")] IReadOnlyList<string> Values)
    : Field(Name, Description, IsArray, IsOptional, DefaultValue);
