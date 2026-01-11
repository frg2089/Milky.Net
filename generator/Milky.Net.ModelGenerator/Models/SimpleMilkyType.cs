using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

public sealed record class SimpleMilkyType(
    string Name,
    string Description,
    [property: JsonPropertyName("fields")] IReadOnlyList<Field> Fields)
    : MilkyType(Name, Description), ISimplyMilkyType;
