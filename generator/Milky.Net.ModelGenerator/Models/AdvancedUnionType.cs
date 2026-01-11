using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

public sealed record class AdvancedUnionType(
    string Name,
    string Description,
    string TagFieldName,
    [property: JsonPropertyName("baseFields")] IReadOnlyList<Field> BaseFields,
    [property: JsonPropertyName("derivedTypes")] IReadOnlyList<MilkyDerivedType> DerivedTypes)
    : UnionMilkyType(Name, Description, TagFieldName);
