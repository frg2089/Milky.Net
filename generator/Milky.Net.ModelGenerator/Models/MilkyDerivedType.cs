using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "derivingType")]
[JsonDerivedType(typeof(SimpleDerivedType), "struct")]
[JsonDerivedType(typeof(RefDerivedType), "ref")]
public abstract record class MilkyDerivedType(
    [property: JsonPropertyName("tagValue")] string TagValue,
    [property: JsonPropertyName("description")] string Description)
    : IDerivableModel;
