using System.Text;

using DotMake.CommandLine;

using Humanizer;

using Milky.Net.SourceGenerator.Common;

namespace Milky.Net.ModelGenerator;

[CliCommand(Parent = typeof(RootCommand))]
internal sealed class GenerateCommand
{
    [CliOption(Required = true)]
    public required FileInfo Input { get; set; }
    [CliOption(Required = true)]
    public required DirectoryInfo Output { get; set; }

    public async Task RunAsync()
    {
        if (!Input.Exists)
            throw new FileNotFoundException("Input file not found", Input.FullName);

        var types = await RootCommand.GetDataAsync(Input);

        if (Output.Exists)
            Output.Delete(true);
        Output.Create();

        using var jsonSerializerContext = File.CreateText(Path.Combine(Output.FullName, "MilkyJsonSerializerContext.g.cs"));
        await jsonSerializerContext.WriteLineAsync("""
            using System.Text.Json;
            using System.Text.Json.Serialization;

            #nullable enable

            namespace Milky.Net.Model;

            [JsonSourceGenerationOptions(WriteIndented = false)]
            [JsonSerializable(typeof(object))]
            [JsonSerializable(typeof(JsonElement))]
            [JsonSerializable(typeof(MilkyResult))]
            """);

        foreach (var type in types)
        {
            var typeName = type.Key;
            var pascalTypeName = typeName.Pascalize();

            if (!pascalTypeName.Contains("<T>"))
                await jsonSerializerContext.WriteLineAsync($"[JsonSerializable(typeof({pascalTypeName}))]");

            var typeInfo = type.Value;
            var path = Path.Combine(Output.FullName, $"{pascalTypeName.Replace("<T>", "{T}")}.g.cs");
            using var writer = File.CreateText(path);

            await writer.WriteLineAsync("""
                using System.Text.Json.Serialization;

                #nullable enable

                namespace Milky.Net.Model;

                """);
            if (!string.IsNullOrWhiteSpace(typeInfo.Description))
                await writer.WriteLineAsync($$"""
                    /// <summary>
                    /// {{typeInfo.Description}}
                    /// </summary>
                    """);

            switch (typeInfo)
            {
                case UnionTypeInfoData:
                case ObjectTypeInfoData:
                    if (typeInfo is IObjectTypeInfoData currentType)
                    {
                        IObjectTypeInfoData? baseType = null;
                        if (!string.IsNullOrEmpty(currentType.BaseType) && types.TryGetValue(currentType.BaseType, out var baseTypeInfo))
                            baseType = baseTypeInfo as IObjectTypeInfoData;

                        foreach (var property in currentType.Properties)
                            await writer.WriteLineAsync($"/// <param name=\"{property.Key.Pascalize()}\">{property.Value.Description}</param>");

                        if (currentType is UnionTypeInfoData unionTypeInfoData)
                        {
                            await writer.WriteLineAsync($"[JsonPolymorphic(TypeDiscriminatorPropertyName = \"{unionTypeInfoData.Discriminator}\")]");
                            foreach (var trueType in unionTypeInfoData.Types)
                                await writer.WriteLineAsync($"[JsonDerivedType(typeof({trueType.Value.Pascalize()}), \"{trueType.Key}\")]");

                            await writer.WriteLineAsync($"public abstract record class {pascalTypeName}(");
                        }
                        else
                        {
                            await writer.WriteLineAsync($"public sealed record class {pascalTypeName}(");
                        }

                        var currentProps = currentType
                            .Properties
                            .Where(i => i.Value.Type is not "literal")
                            .Select(PropertyInfo.Create);

                        var baseProps = baseType
                            ?.Properties
                            .Where(i => i.Value.Type is not "literal")
                            .Select(PropertyInfo.Create)
                            .Select(i => i.PropertyName)
                            .ToList() ?? [];

                        var props = currentProps.Select(prop =>
                        {
                            StringBuilder sb = new("    ");
                            if (!baseProps.Contains(prop.PropertyName))
                                sb.Append(prop.JsonAttributes).Append(' ');

                            sb.Append(prop.ParameterDeclaration);
                            return sb.ToString();
                        });

                        var sortedProps = props
                            .Where(i => !i.Contains('='))
                            .Concat(props.Where(i => i.Contains('=')));

                        await writer.WriteAsync(string.Join($",{writer.NewLine}", sortedProps));

                        if (!string.IsNullOrEmpty(currentType.BaseType))
                        {
                            await writer.WriteAsync($") : {currentType.BaseType.Pascalize()}({writer.NewLine}    ");
                            await writer.WriteAsync(string.Join($",{writer.NewLine}    ", baseProps));
                        }
                        await writer.WriteLineAsync(");");
                    }
                    break;
                case GenericTypeInfoData genericTypeInfoData when types[genericTypeInfoData.BaseType] is IObjectTypeInfoData baseType:
                    {
                        var baseProps = baseType
                            .Properties
                            .Where(i => i.Value.Type is not "literal")
                            .Select(PropertyInfo.Create);
                        var props = baseProps
                            .Select(prop => prop.ParameterDeclaration);

                        await writer.WriteLineAsync($"/// <inheritdoc cref=\"{genericTypeInfoData.BaseType.Pascalize()}\"/>");
                        await writer.WriteLineAsync($"public sealed record class {pascalTypeName}(");
                        foreach (var prop in props.Where(i => !i.Contains('=')))
                            await writer.WriteLineAsync($"    {prop},");

                        await writer.WriteAsync($"    [property: JsonPropertyName(\"{genericTypeInfoData.GenericPropertyName}\")] T {genericTypeInfoData.GenericPropertyName.Pascalize()}");
                        foreach (var prop in props.Where(i => i.Contains('=')))
                            await writer.WriteAsync($",{writer.NewLine}    {prop}");

                        await writer.WriteLineAsync();
                        await writer.WriteLineAsync($") : {genericTypeInfoData.BaseType.Pascalize()}(");
                        await writer.WriteLineAsync(string.Join($",{writer.NewLine}", baseProps
                            .Select(prop => $"    {prop.JsonPropertyName.Pascalize()}")));
                        await writer.WriteLineAsync(");");
                    }
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
    }
}

file sealed record class PropertyInfo(string TypeName, string JsonPropertyName, string? DefaultValue)
{
    public string TypeName { get; } = ParseTypeName(TypeName, JsonPropertyName);
    public string PropertyName { get; } = JsonPropertyName.Pascalize();

    public string JsonAttributes
    {
        get
        {
            var result = $"[property: JsonPropertyName(\"{JsonPropertyName}\")]";
            if (TypeName.Contains("DateTimeOffset"))
                result += "[property: JsonConverter(typeof(SecondTimestampDateTimeOffsetJsonConverter))]";

            return result;
        }
    }

    public string ParameterDeclaration
    {
        get
        {
            var result = $"{TypeName} {PropertyName}";
            if (DefaultValue is not null)
                result += $" = {DefaultValue}";

            return result;
        }
    }

    public static PropertyInfo Create(KeyValuePair<string, PropertyInfoData> property)
    {
        string? defaultValue = null;
        var typeName = property.Value.Type;
        if (typeName.Contains('='))
        {
            var tmp = typeName.Split('=', 2, StringSplitOptions.TrimEntries);
            typeName = tmp[0];
            defaultValue = tmp[1];
        }
        return new(typeName, property.Key, defaultValue);
    }

    private static string ParseTypeName(string typeName, string propertyName)
    {
        var isNullable = typeName.EndsWith('?');
        if (isNullable)
            typeName = typeName[..^1];

        typeName = typeName switch
        {
            "string" when propertyName.Contains("uri", StringComparison.OrdinalIgnoreCase) => "MilkyUri",
            "string" => "string",
            "boolean" => "bool",
            "int32" or "Int32" => "int",
            "int64" or "Int64" when propertyName.Contains("time", StringComparison.OrdinalIgnoreCase) => "DateTimeOffset",
            "int64" or "Int64" => "long",
            "uin" or "Uin" => "long",
            _ => typeName.Pascalize()
        };

        return isNullable ? $"{typeName}?" : typeName;
    }
}