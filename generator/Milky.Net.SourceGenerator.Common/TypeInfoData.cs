using System.Text.Json.Serialization;

namespace Milky.Net.SourceGenerator.Common;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ObjectTypeInfoData), "object")]
[JsonDerivedType(typeof(UnionTypeInfoData), "union")]
[JsonDerivedType(typeof(EnumTypeInfoData), "enum")]
public abstract record TypeInfoData(
    [property: JsonPropertyName("description")] string? Description
);
