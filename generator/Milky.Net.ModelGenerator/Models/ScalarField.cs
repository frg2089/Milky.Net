using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

public sealed record class ScalarField(
    string Name,
    string Description,
    bool? IsArray,
    bool? IsOptional,
    JsonElement? DefaultValue,
    [property: JsonPropertyName("scalarType")] string ScalarType)
    : Field(Name, Description, IsArray, IsOptional, DefaultValue);
