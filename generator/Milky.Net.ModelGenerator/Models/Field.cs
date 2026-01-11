using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "fieldType")]
[JsonDerivedType(typeof(ScalarField), "scalar")]
[JsonDerivedType(typeof(EnumField), "enum")]
[JsonDerivedType(typeof(RefField), "ref")]
public abstract record class Field(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("isArray")] bool? IsArray,
    [property: JsonPropertyName("isOptional")] bool? IsOptional,
    [property: JsonPropertyName("defaultValue")] JsonElement? DefaultValue);
