using System.Collections.Immutable;
using System.Text;

using Humanizer;

using Milky.Net.ModelGenerator.Models;

namespace Milky.Net.ModelGenerator;

internal static class MilkyCSharpModelTypeGenerator
{
    public static IEnumerable<TypeBuilder> Parse(MilkyIR ir)
    {
        IEnumerable<MilkyType> types = ir.CommonStructs;

        var specialUnionTypes = types
            .OfType<AdvancedUnionType>()
            .SelectMany(static i => i.DerivedTypes
                .OfType<RefDerivedType>()
                .Select(t => (i.Name, t.RefStructName)))
            .GroupBy(static i => i.Name)
            .ToImmutableDictionary(
                static i => i.Key,
                static i => i.Select(i => i.RefStructName));

        var refTypes = specialUnionTypes.SelectMany(i => i.Value);

        types = types.Where(i => !refTypes.Contains(i.Name));

        foreach (var type in types)
        {
            var list = ParseType(type.Name, type);

            foreach (var item in list)
                yield return item;
        }

        foreach (var item in ir.CommonStructs.Where(i => refTypes.Contains(i.Name)))
        {
            foreach (var code in ParseType(item.Name.Pascalize(), item))
                yield return code;
        }

        foreach (var item in ParseApiCategories(ir.ApiCategories))
            yield return item;
    }

    private static IEnumerable<TypeBuilder> ParseType(
        string typeName,
        IMilkyType type,
        ModelClassBuilder? baseType = null) => type switch
        {
            ISimplyMilkyType simple => ParseSimpleType(typeName, simple, baseType),
            SimpleUnionType union => ParseSimpleUnion(typeName, union, baseType),
            AdvancedUnionType union => ParseWithDataUnion(typeName, union, baseType),
            RefDerivedType => [],
            _ => throw new InvalidCastException(),
        };

    private static IEnumerable<TypeBuilder> ParseSimpleType(
        string typeName,
        ISimplyMilkyType type,
        ModelClassBuilder? baseType = null) => ParseSimpleTypeCore(typeName, type, type.Fields, baseType);
    private static IEnumerable<TypeBuilder> ParseSimpleType(
        string typeName,
        Api type,
        IEnumerable<Field> fields,
        ModelClassBuilder? baseType = null) => ParseSimpleTypeCore(typeName, type, fields, baseType);
    private static IEnumerable<TypeBuilder> ParseSimpleTypeCore(
        string typeName,
        IMilkyType type,
        IEnumerable<Field> fields,
        ModelClassBuilder? baseType = null)
    {
        typeName = typeName.Pascalize();
        var parameters = fields.ConcatBase(baseType?.Params ?? []).SortFields();

        ModelClassBuilder modelClassBuilder = new()
        {
            Name = typeName,
            Description = type.Description,
            Params = [.. parameters],
            Inherit = baseType,
        };
        yield return modelClassBuilder;

        foreach (var field in ParseEnums(parameters))
            yield return field;
    }



    private static IEnumerable<TypeBuilder> ParseSimpleUnion(
        string unionTypeName,
        SimpleUnionType union,
        ModelClassBuilder? baseType = null)
    {
        IEnumerable<Field> unionFields = (baseType?.Params ?? []).SortFields();

        var converterTypeName = $"{unionTypeName}JsonConverter";

        ModelClassBuilder modelClassBuilder = new()
        {
            Name = unionTypeName,
            Description = union.Description,
            Params = [.. unionFields],
            Attributes = [
                $"[global::System.Text.Json.Serialization.JsonConverter(typeof({converterTypeName}))]"
            ],
            Inherit = baseType,
            IsAbstract = true,
        };

        yield return modelClassBuilder;

        Dictionary<string, string> derivedStricts = new(union.DerivedStructs.Count);
        foreach (var type in union.DerivedStructs)
        {
            derivedStricts[type.TagValue] = $"{type.TagValue}_{union.Name}".Pascalize();
            foreach (var result in ParseSimpleType(derivedStricts[type.TagValue], type, modelClassBuilder))
                yield return result;
        }

        yield return GenerateJsonConverter(converterTypeName, union.TagFieldName, modelClassBuilder, derivedStricts);
    }

    private static JsonConverterBuilder GenerateJsonConverter(
        string converterTypeName,
        string typeDiscriminatorPropertyName,
        ModelClassBuilder union,
        Dictionary<string, string> derivedTypes,
        bool withData = false) => new()
        {
            Name = converterTypeName,
            Description = union.Description,
            TargetType = union,
            TypeDiscriminatorPropertyName = typeDiscriminatorPropertyName,
            DerivedTypes = derivedTypes,
            WithData = withData,
        };

    private static IEnumerable<TypeBuilder> ParseWithDataUnion(
        string unionTypeName,
        AdvancedUnionType union,
        ModelClassBuilder? baseType = null)
    {
        var unionFields = union.BaseFields.ConcatBase(baseType?.Params ?? [])
            .SortFields(union.TagFieldName);

        var converterTypeName = $"{unionTypeName}JsonConverter";

        // 构造非泛型抽象类
        ModelClassBuilder baseCodeBuilder = new()
        {
            Name = unionTypeName,
            Description = union.Description,
            Params = new(unionFields.Count()),
            Attributes = [
                $"// [global::System.Text.Json.Serialization.JsonPolymorphic(TypeDiscriminatorPropertyName = \"{union.TagFieldName}\")]",
                $"[global::System.Text.Json.Serialization.JsonConverter(typeof({converterTypeName}))]",
            ],
            IsAbstract = true,
        };

        Dictionary<string, string> derivedTypes = new(union.DerivedTypes.Count);
        foreach (var item in union.DerivedTypes)
        {
            var typeName = item is RefDerivedType refDerivedType
                ? refDerivedType.RefStructName
                : $"{item.TagValue}_{union.Name}_data";

            typeName = $"{unionTypeName}<{typeName.Pascalize()}>";

            derivedTypes[item.TagValue] = typeName;

            baseCodeBuilder.Attributes.Add($"// [global::System.Text.Json.Serialization.JsonDerivedType(typeof({typeName}), \"{item.TagValue}\")]");
        }

        if (unionFields.FirstOrDefault(i => i.Name == union.TagFieldName) is ScalarField
            {
                IsArray: not true,
                IsOptional: not true,
                DefaultValue: null
            } tagField)
        {
            baseCodeBuilder.Params.Add(tagField);

            unionFields = unionFields.Where(i => i != tagField);
        }

        foreach (var field in unionFields.Where(i => i.DefaultValue is null))
            baseCodeBuilder.Params.Add(field);

        yield return baseCodeBuilder;

        var types = union.DerivedTypes.Select(i =>
        {
            var typeName = i switch
            {
                RefDerivedType refDerivedType => refDerivedType.RefStructName.Pascalize(),
                _ => $"{i.TagValue}_{union.Name}_data".Pascalize(),
            };
            return (TypeName: typeName, Type: i);
        });

        // 构造泛型类
        ModelClassBuilder genericCodeBuilder = baseCodeBuilder with
        {
            IsAbstract = false,
            TypeParams = [
                ("Data", "Data type", [..types.Select(static i => i.TypeName)]),
            ],
            Inherit = baseCodeBuilder,
        };
        yield return genericCodeBuilder;

        foreach (var field in ParseEnums(unionFields))
            yield return field;

        foreach (var item in types.SelectMany(static i => ParseType(i.TypeName, i.Type)))
            yield return item;

        yield return GenerateJsonConverter(converterTypeName, union.TagFieldName, baseCodeBuilder, derivedTypes, true);
    }

    private static IEnumerable<EnumBuilder> ParseEnums(IEnumerable<Field> fields)
    {
        foreach (var item in fields.OfType<EnumField>())
        {
            var typeName = item.Name.Pascalize();
            yield return new()
            {
                Name = typeName,
                Description = item.Description,
                Enum = item,
            };
        }
    }

    private static IEnumerable<TypeBuilder> ParseApi(Api api)
    {
        if (api.RequestFields is { Count: not 0 })
        {
            var typeName = $"{api.Endpoint}_request".Pascalize();
            foreach (var item in ParseSimpleType(typeName, api, api.RequestFields))
                yield return item;

        }
        if (api.ResponseFields is { Count: not 0 })
        {
            var typeName = $"{api.Endpoint}_response".Pascalize();
            foreach (var item in ParseSimpleType(typeName, api, api.ResponseFields))
                yield return item;
        }
    }

    private static IEnumerable<TypeBuilder> ParseApiCategories(IEnumerable<ApiCategory> categories)
    {
        foreach (var item in categories.SelectMany(static i => i.Apis.SelectMany(ParseApi)))
            yield return item;
    }

    private static IEnumerable<Field> ConcatBase(this IEnumerable<Field> fields, IEnumerable<Field> baseFields)
    {
        var names = baseFields.Select(i => i.Name).ToArray();
        foreach (var field in fields)
        {
            if (!names.Contains(field.Name))
                yield return field;
        }

        foreach (var field in baseFields)
            yield return field;
    }

    /// <summary>
    /// 排序字段
    /// </summary>
    /// <param name="fields"></param>
    /// <param name="tagFieldName">需要放在首位的字段</param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    private static IEnumerable<Field> SortFields(this IEnumerable<Field> fields, string? tagFieldName = null)
    {
        if (tagFieldName is { Length: not 0 })
        {
            var tagField = fields.FirstOrDefault(i => i.Name == tagFieldName);
            if (tagField is { IsArray: not true, IsOptional: not true, DefaultValue: null })
            {
                fields = fields.Where(i => i != tagField);

                yield return tagField;
            }
        }

        foreach (var item in fields.Where(static i => !i.DefaultValue.HasValue))
            yield return item;

        foreach (var item in fields.Where(static i => i.DefaultValue.HasValue))
            yield return item;
    }

    extension(StringBuilder builder)
    {
        public StringBuilder AppendSummary(IMilkyType type)
            => builder.AppendLine($"""
            /// <summary>
            /// {type.Description}
            /// </summary>
            """);

        private StringBuilder AppendConstructorArgumentList(IEnumerable<Field> fields, IEnumerable<Field>? baseFields = default)
        {
            baseFields ??= [];

            List<string> codes = [];
            foreach (var field in fields)
            {
                var isBase = baseFields.Contains(field);

                string code = isBase
                    ? ParseFieldWithParameter(field)
                    : ParseFieldWithPropertyParameter(field);

                codes.Add(code);
            }

            var parameters = codes
                .Select(static i => $"\r\n    {i}");

            builder.Append(parameters.Join(','));

            return builder;
        }

        private StringBuilder AppendBaseConstructorArgumentList(IEnumerable<Field> fields)
        {
            var codes = fields.Select(i => i.Name.Pascalize());

            var parameters = codes
                .Select(static i => $"\r\n    {i}");

            builder.Append(parameters.Join(','));

            return builder;
        }

        public StringBuilder AppendParam(Field field)
            => builder.AppendLine($"/// <param name=\"{field.Name.Pascalize()}\">{field.Description}</param>");

        public StringBuilder AppendParams(IEnumerable<Field> fields)
        {
            foreach (var field in fields)
                builder.AppendParam(field);

            return builder;
        }

        public StringBuilder AppendTypeParameter(string typeParameterName, string description)
            => builder.AppendLine($"/// <typeparam name=\"{typeParameterName}\">{description}</typeparam>");

        public StringBuilder AppendTypeDefine(string typeName, bool isAbstract = false)
            => builder.Append($"public {(isAbstract ? "abstract" : "sealed")} record class {typeName}");

        public StringBuilder AppendBaseType(string baseType)
            => builder.Append($"\r\n    : {baseType}");
    }

    private static string ParseFieldWithPropertyParameter(Field field)
    {
        StringBuilder sb = new($"[property: global::System.Text.Json.Serialization.JsonPropertyName(\"{field.Name}\")]");
        if (field is ScalarField { ScalarType: "int64" } scalar && scalar.Name.Contains("time", StringComparison.OrdinalIgnoreCase))
            sb.Append("[property: global::System.Text.Json.Serialization.JsonConverter(typeof(global::Milky.Net.Model.SecondTimestampDateTimeOffsetJsonConverter))]");

        sb.Append(' ');
        sb.Append(ParseFieldWithParameter(field));

        return sb.ToString();
    }

    private static string ParseFieldWithParameter(Field field)
    {
        StringBuilder sb = new();
        switch (field)
        {
            case ScalarField scalar:
                sb.Append(ParseScalarType(scalar.ScalarType, scalar.Name));
                break;
            case EnumField @enum:
                sb.Append(@enum.Name.Pascalize());
                break;
            case RefField @ref:
                sb.Append(@ref.RefStructName.Pascalize());
                break;
            default:
                throw new InvalidCastException();
        }

        if (field.IsArray is true)
            sb.Append("[]");
        if (field.IsOptional is true)
            sb.Append('?');
        sb.Append(' ');
        sb.Append(field.Name.Pascalize());

        if (field.DefaultValue.HasValue)
        {
            if (field is EnumField @enum)
            {
                sb.Append($" = {@enum.Name.Pascalize()}.{field.DefaultValue?.GetString()?.Pascalize()}");
            }
            else
            {
                sb.Append($" = {field.DefaultValue?.GetRawText()}");
            }
        }

        return sb.ToString();
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
