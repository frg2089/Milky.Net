
using Humanizer;

using Milky.Net.ModelGenerator.Models;

namespace Milky.Net.ModelGenerator;

internal abstract record class TypeBuilder
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<string> Attributes { get; init; } = [];
    public abstract string Build();
}

internal sealed record class EnumBuilder : TypeBuilder
{
    public required EnumField Enum { get; init; }

    public override string Build()
    {
        var values = Enum.Values.Select(static i => $"""
                [global::System.Text.Json.Serialization.JsonStringEnumMemberName("{i}")]
                {i.Pascalize()},
            """);
        return $$"""
            /// <summary>
            /// {{Description}}
            /// </summary>
            [global::System.Text.Json.Serialization.JsonConverter(typeof(global::System.Text.Json.Serialization.JsonStringEnumConverter<{{Name}}>))]
            public enum {{Name}}
            {
            {{string.Join("\r\n", values)}}
            }
            """;
    }
}

internal sealed record class ModelClassBuilder : TypeBuilder
{
    public List<(string Name, string Description, List<string> PosableTypes)> TypeParams { get; init; } = [];
    public List<Field> Params { get; init; } = [];
    public bool IsAbstract { get; set; }
    public ModelClassBuilder? Inherit { get; set; }

    public override string Build()
    {
        using StringWriter writer = new();
        if (Description is { Length: not 0 })
            writer.WriteLine(
                $$"""
                /// <summary>
                /// {{Description}}
                /// </summary>
                """);
        foreach (var (typeParameterName, description, types) in TypeParams)
        {
            writer.WriteLine($"/// <typeparam name=\"T{typeParameterName.Pascalize()}\">");
            writer.WriteLine($"/// {description}<br />");

            writer.WriteLine($"/// <list type=\"bullet\">");
            foreach (var type in types)
                writer.WriteLine($"///     <item><see cref=\"{type}\"/></item>");
            writer.WriteLine($"/// </list>");

            writer.WriteLine($"/// </typeparam>");
        }
        foreach (var param in Params)
            writer.WriteLine($"/// <param name=\"{param.Name.Pascalize()}\">{param.Description}</param>");

        foreach (var attribute in Attributes)
            writer.WriteLine(attribute);

        writer.Write("public");
        if (IsAbstract)
            writer.Write(" abstract");
        else
            writer.Write(" sealed");
        writer.Write(" partial record class ");
        writer.Write(Name);
        if (TypeParams.Count is not 0)
        {
            writer.Write("<");
            writer.Write(string.Join(", ", TypeParams.Select(static i => $"T{i.Name.Pascalize()}")));
            writer.Write(">");
        }
        writer.WriteLine('(');

        bool hasParams = false;
        var requiredParams = Params.Where(i => i is not { IsOptional: true } and not { DefaultValue: not null });
        foreach (var field in requiredParams)
        {
            hasParams = true;
            writer.Write("    ");
            if (Inherit?.Params.Contains(field) is true)
                ParseFieldAsParameter(field, writer);
            else
                ParseFieldAsProperty(field, writer);

            writer.Write(',');
            writer.WriteLine();
        }

        foreach (var field in TypeParams)
        {
            hasParams = true;
            writer.Write("    ");
            writer.Write($"[property: global::System.Text.Json.Serialization.JsonPropertyName(\"{field.Name}\")]");
            writer.WriteLine($"T{field.Name.Pascalize()} {field.Name.Pascalize()},");
        }

        var optionalParams = Params.Where(i => i is { IsOptional: true } or { DefaultValue: not null });
        foreach (var field in optionalParams)
        {
            hasParams = true;
            writer.Write("    ");
            if (Inherit?.Params.Contains(field) is true)
                ParseFieldAsParameter(field, writer);
            else
                ParseFieldAsProperty(field, writer);

            writer.Write(',');
            writer.WriteLine();
        }
        if (hasParams)
            writer.GetStringBuilder().Length -= writer.NewLine.Length + 1;

        writer.Write(')');
        if (Inherit is not null)
        {
            writer.Write(" : ");
            writer.Write(Inherit.Name);
            if (Inherit.TypeParams.Count is not 0)
                throw new InvalidOperationException();

            writer.WriteLine('(');
            for (var i = 0; i < Inherit.Params.Count; i++)
            {
                if (i is not 0)
                    writer.Write(',');

                writer.Write("    ");
                writer.Write(Inherit.Params[i].Name.Pascalize());

                writer.WriteLine();
            }

            writer.Write(')');
        }
        writer.Write(';');

        return writer.ToString();
    }

    private static void ParseFieldAsProperty(Field field, StringWriter writer)
    {
        writer.Write($"[property: global::System.Text.Json.Serialization.JsonPropertyName(\"{field.Name}\")]");
        if (field is ScalarField { ScalarType: "int64" } scalar && scalar.Name.Contains("time", StringComparison.OrdinalIgnoreCase))
            writer.Write("[property: global::System.Text.Json.Serialization.JsonConverter(typeof(global::Milky.Net.Model.SecondTimestampDateTimeOffsetJsonConverter))]");

        writer.Write(' ');
        ParseFieldAsParameter(field, writer);
    }
    private static void ParseFieldAsParameter(Field field, StringWriter writer)
    {
        switch (field)
        {
            case ScalarField scalar:
                writer.Write(ParseScalarType(scalar.ScalarType, scalar.Name));
                break;
            case EnumField @enum:
                writer.Write(@enum.Name.Pascalize());
                break;
            case RefField @ref:
                writer.Write(@ref.RefStructName.Pascalize());
                break;
            default:
                throw new InvalidCastException();
        }

        if (field.IsArray is true)
            writer.Write("[]");
        if (field.IsOptional is true)
            writer.Write('?');
        writer.Write(' ');
        writer.Write(field.Name.Pascalize());

        if (field.DefaultValue.HasValue)
        {
            if (field is EnumField @enum)
                writer.Write($" = {@enum.Name.Pascalize()}.{field.DefaultValue?.GetString()?.Pascalize()}");
            else
                writer.Write($" = {field.DefaultValue?.GetRawText()}");
        }
    }

    private static string ParseScalarType(string scalarType, string name) => scalarType switch
    {
        "int32" => "int",
        "int64" when name.Contains("time", StringComparison.OrdinalIgnoreCase) => "global::System.DateTimeOffset",
        "int64" => "long",
        "bool" => "bool",
        "string" when name.Contains("uri", StringComparison.OrdinalIgnoreCase) => "global::Milky.Net.Model.MilkyUri",
        "string" => "string",
        _ => throw new InvalidCastException(),
    };
}

internal sealed record class JsonConverterBuilder : TypeBuilder
{
    public required ModelClassBuilder TargetType { get; init; }
    public required string TypeDiscriminatorPropertyName { get; init; }
    public required Dictionary<string, string> DerivedTypes { get; init; } = [];
    public bool WithData { get; set; }

    public override string Build()
    {
        using StringWriter readSwitchArms = new();
        using StringWriter writeSwitchArms = new();

        foreach (var derived in DerivedTypes)
        {
            var typeName = $"{derived.Value}";
            readSwitchArms.WriteLine($"            \"{derived.Key}\" => json.Deserialize(options.GetTypeInfo<{typeName}>()),");
            writeSwitchArms.WriteLine($"            {typeName} derived => SerializeDerived<{typeName}>(derived, \"{derived.Key}\", options),");
        }
        readSwitchArms.WriteLine($"            _ => throw new global::System.Text.Json.JsonException($\"Unknown {TypeDiscriminatorPropertyName}: '{{tag}}'.\"),");
        writeSwitchArms.WriteLine($"            _ => throw new global::System.Text.Json.JsonException($\"Unknown derived type: '{{value.GetType().Name}}'.\"),");

        return $$"""
            using global::System.Text.Json;

            public sealed class {{Name}} : global::System.Text.Json.Serialization.JsonConverter<{{TargetType.Name}}>
            {
                public override {{TargetType.Name}}? Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
                {
                    var doc = global::System.Text.Json.JsonDocument.ParseValue(ref reader);
                    var json = doc.RootElement;
                    
                    var tag = json.GetProperty("{{TypeDiscriminatorPropertyName}}").GetString()
                        ?? throw new global::System.Text.Json.JsonException("Missing discriminator '{{TypeDiscriminatorPropertyName}}'.");

                    return tag switch
                    {
            {{readSwitchArms}}
                    };
                }

                public override void Write(global::System.Text.Json.Utf8JsonWriter writer, {{TargetType.Name}} value, global::System.Text.Json.JsonSerializerOptions options)
                {
                    var node = value switch
                    {
            {{writeSwitchArms}}
                    };

                    node.WriteTo(writer, options);
                }

                private static global::System.Text.Json.Nodes.JsonObject SerializeDerived<T>(T value, string tagValue, global::System.Text.Json.JsonSerializerOptions options)
                {
                    var node = global::System.Text.Json.JsonSerializer.SerializeToNode(value, options.GetTypeInfo<T>())?.AsObject()
                        ?? throw new global::System.Text.Json.JsonException("Serialization produced null.");

                    node["{{TypeDiscriminatorPropertyName}}"] = tagValue;
                    return node;
                }
            }
            """;
    }
}
