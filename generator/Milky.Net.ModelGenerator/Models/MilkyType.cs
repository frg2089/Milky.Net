using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "structType")]
[JsonDerivedType(typeof(SimpleMilkyType), "simple")]
[JsonDerivedType(typeof(UnionMilkyType), "union")]
public abstract record class MilkyType(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string Description)
    : IMilkyType;
