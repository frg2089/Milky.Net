using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

public sealed record class SimpleUnionType(
    string Name,
    string Description,
    string TagFieldName,
    [property: JsonPropertyName("derivedStructs")] IReadOnlyList<SimpleDerivedType> DerivedStructs)
    : UnionMilkyType(Name, Description, TagFieldName);
