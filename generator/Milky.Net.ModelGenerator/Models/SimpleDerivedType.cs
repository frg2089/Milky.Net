using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

public sealed record class SimpleDerivedType(
    string TagValue,
    string Description,
    [property: JsonPropertyName("fields")] IReadOnlyList<Field> Fields)
    : MilkyDerivedType(TagValue, Description), ISimplyMilkyType;
