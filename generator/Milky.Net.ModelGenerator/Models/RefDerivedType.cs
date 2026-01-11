using System.Text.Json.Serialization;

namespace Milky.Net.ModelGenerator.Models;

public sealed record class RefDerivedType(
    string TagValue,
    string Description,
    [property: JsonPropertyName("refStructName")] string RefStructName)
    : MilkyDerivedType(TagValue, Description);
