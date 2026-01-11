using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

public sealed record class RefField(
    string Name,
    string Description,
    bool? IsArray,
    bool? IsOptional,
    JsonElement? DefaultValue,
    [property: JsonPropertyName("refStructName")] string RefStructName)
    : Field(Name, Description, IsArray, IsOptional, DefaultValue);
