namespace Milky.Net.SourceGenerator.Common;

internal interface IObjectTypeInfoData
{
    string? Description { get; }
    Dictionary<string, PropertyInfoData> Properties { get; }
    string? BaseType { get; }
}