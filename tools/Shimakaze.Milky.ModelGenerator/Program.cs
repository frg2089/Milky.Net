using System.Diagnostics;
using System.Text;
using System.Text.Json;

using Humanizer;

using Shimakaze.Milky.ModelGenerator;

var inputPath = args[0];
var outputPath = args[1];

Directory.CreateDirectory(outputPath);

await using var fs = File.OpenRead(inputPath);
var types = await JsonSerializer.DeserializeAsync<Dictionary<string, TypeInfoData>>(fs);
Debug.Assert(types is not null);

using var jsonSerializerContext = File.CreateText(Path.Combine(outputPath, "MilkyJsonSerializerContext.cs"));
await jsonSerializerContext.WriteLineAsync("""
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model;

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(MilkyResult))]
""");

foreach (var type in types)
{
    var typeName = type.Key;
    var pascalTypeName = typeName.Pascalize();

    await jsonSerializerContext.WriteLineAsync($"[JsonSerializable(typeof({pascalTypeName}))]");

    var typeInfo = type.Value;
    var path = Path.Combine(outputPath, $"{pascalTypeName}.cs");
    using var writer = File.CreateText(path);

    await writer.WriteLineAsync($$"""
        using System.Text.Json.Serialization;

        namespace Shimakaze.Milky.Model;

        /// <summary>
        /// {{typeInfo.Description}}
        /// </summary>
        """);
    switch (typeInfo)
    {
        case ObjectTypeInfoData objectTypeInfoData:
            foreach (var property in objectTypeInfoData.Properties)
                await writer.WriteLineAsync($"/// <param name=\"{property.Key.Pascalize()}\">{property.Value.Description}</param>");

            await writer.WriteLineAsync($"public sealed record class {pascalTypeName}(");

            var props = objectTypeInfoData
                .Properties
                .Where(i => i.Value.Type is not "literal")
                .Select(property =>
                {
                    string? defaultValue = null;
                    var typeName = property.Value.Type;
                    if (typeName.Contains('='))
                    {
                        var tmp = typeName.Split('=', 2, StringSplitOptions.TrimEntries);
                        typeName = tmp[0];
                        defaultValue = tmp[1];
                    }
                    typeName = ParseTypeName(typeName);
                    if (typeName == "long" && property.Key.Contains("time", StringComparison.OrdinalIgnoreCase))
                        typeName = "DateTimeOffset";
                    else if (typeName == "long?" && property.Key.Contains("time", StringComparison.OrdinalIgnoreCase))
                        typeName = "DateTimeOffset?";
                    else if (typeName == "string" && property.Key.Contains("uri", StringComparison.OrdinalIgnoreCase))
                        typeName = "Uri";
                    else if (typeName == "string?" && property.Key.Contains("uri", StringComparison.OrdinalIgnoreCase))
                        typeName = "Uri?";

                    StringBuilder sb = new();
                    sb
                        .Append("    [property: JsonPropertyName(")
                        .Append('"')
                        .Append(property.Key)
                        .Append('"');
                    if (typeName.Contains("DateTimeOffset"))
                        sb.Append(")][property: JsonConverter(typeof(SecondTimestampDateTimeOffsetJsonConverter)");

                    sb
                        .Append(")] ")
                        .Append(typeName)
                        .Append(' ')
                        .Append(property.Key.Pascalize());

                    if (defaultValue is not null)
                        sb
                            .Append(" = ")
                            .Append(defaultValue);


                    return sb.ToString();
                });

            var sortedProps = props
                .Where(i => !i.Contains('='))
                .Concat(props.Where(i => i.Contains('=')));

            await writer.WriteAsync(string.Join(",\n", sortedProps) + ")");

            if (!string.IsNullOrEmpty(objectTypeInfoData.BaseType))
                await writer.WriteAsync($" : {objectTypeInfoData.BaseType.Pascalize()}");

            await writer.WriteLineAsync(";");

            break;
        case UnionTypeInfoData unionTypeInfoData:
            await writer.WriteLineAsync($"[JsonPolymorphic(TypeDiscriminatorPropertyName = \"{unionTypeInfoData.Discriminator}\")]");
            foreach (var trueType in unionTypeInfoData.Types)
                await writer.WriteLineAsync($"[JsonDerivedType(typeof({trueType.Value.Pascalize()}), \"{trueType.Key}\")]");

            await writer.WriteLineAsync($"public abstract record class {pascalTypeName};");

            break;
        case EnumTypeInfoData enumTypeInfoData:
            if (!enumTypeInfoData.Items.FirstOrDefault().Value.Number.HasValue)
                await writer.WriteLineAsync($"[JsonConverter(typeof(JsonStringEnumConverter<{pascalTypeName}>))]");

            await writer.WriteLineAsync($"public enum {pascalTypeName}");
            await writer.WriteLineAsync("{");

            foreach (var item in enumTypeInfoData.Items)
            {
                if (!item.Value.Number.HasValue)
                    await writer.WriteLineAsync($"    [JsonStringEnumMemberName(\"{item.Key}\")]");
                await writer.WriteAsync("    ");
                await writer.WriteAsync(item.Key.Pascalize());
                if (item.Value.Number.HasValue)
                    await writer.WriteAsync($" = {item.Value.Number}");
                await writer.WriteLineAsync(",");
            }

            await writer.WriteLineAsync("}");
            break;
    }
}

await jsonSerializerContext.WriteLineAsync("""
public sealed partial class MilkyJsonSerializerContext : JsonSerializerContext
{
}
""");

static string ParseTypeName(string typeName) => typeName switch
{
    "string" => "string",
    "boolean" => "bool",
    "Int32" => "int",
    "Int64" => "long",
    "string?" => "string?",
    "boolean?" => "bool?",
    "Int32?" => "int?",
    "Int64?" => "long?",
    _ => typeName.Pascalize()
};