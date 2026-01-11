using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "unionType")]
[JsonDerivedType(typeof(SimpleUnionType), "plain")]
[JsonDerivedType(typeof(AdvancedUnionType), "withData")]
public abstract record class UnionMilkyType(
    string Name,
    string Description,
    [property: JsonPropertyName("tagFieldName")] string TagFieldName)
    : MilkyType(Name, Description);
