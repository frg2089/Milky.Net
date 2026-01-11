using System.Collections.Immutable;
using System.Text;

using Humanizer;

using Milky.Net.ModelGenerator.Models;

namespace Milky.Net.ModelGenerator;

internal static class MilkyCSharpModelTypeGenerator
{
    public static IEnumerable<(string TypeName, string TypeCode)> Parse(MilkyIR ir)
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
            var baseTypeName = specialUnionTypes.First(i => i.Value.Contains(item.Name)).Key;
            var union = ir.CommonStructs
                .OfType<AdvancedUnionType>()
                .First(i => i.Name == baseTypeName);

            baseTypeName = baseTypeName.Pascalize();

            var currentType = item.Name.Pascalize();

            foreach (var code in ParseType(currentType, item, baseTypeName, union.BaseFields))
                yield return code;
        }

        foreach (var item in ParseApiCategories(ir.ApiCategories))
            yield return item;
    }

    private static IEnumerable<(string TypeName, string TypeCode)> ParseType(
        string typeName,
        IMilkyType type,
        string? baseTypeName = null,
        IEnumerable<Field>? baseFields = null) => type switch
        {
            ISimplyMilkyType simple => ParseSimpleType(typeName, simple, simple.Fields, baseTypeName, baseFields),
            SimpleUnionType union => ParseSimpleUnion(typeName, union, baseTypeName, baseFields),
            AdvancedUnionType union => ParseWithDataUnion(typeName, union, baseTypeName, baseFields),
            RefDerivedType => [],
            _ => throw new InvalidCastException(),
        };

    private static IEnumerable<(string TypeName, string TypeCode)> ParseSimpleType(
        string typeName,
        IMilkyType type,
        IEnumerable<Field> fields,
        string? baseTypeName = null,
        IEnumerable<Field>? baseFields = null)
    {
        typeName = typeName.Pascalize();
        var parameters = fields.ConcatBase(baseFields ?? []).SortFields();

        StringBuilder builder = new();
        builder
            .AppendSummary(type)
            .AppendParams(parameters)
            .AppendTypeDefine(typeName)
            .Append('(')
            .AppendConstructorArgumentList(parameters, baseFields)
            .Append(')');

        if (baseTypeName is { Length: not 0 })
        {
            builder
                .AppendBaseType(baseTypeName)
                .Append('(');
            if (baseFields is not null)
                builder.AppendBaseConstructorArgumentList(baseFields);
            builder.Append(')');
        }

        builder.Append(';');

        yield return (typeName, builder.ToString());

        foreach (var field in ParseEnums(parameters))
            yield return field;
    }

    private static IEnumerable<(string TypeName, string TypeCode)> ParseSimpleUnion(
        string unionTypeName,
        SimpleUnionType union,
        string? baseTypeName = null,
        IEnumerable<Field>? baseFields = null)
    {
        IEnumerable<Field> unionFields = (baseFields ?? []).SortFields();

        StringBuilder builder = new();
        builder
            .AppendSummary(union)
            .AppendParams(unionFields)
            .AppendJsonPolymorphic(union.TagFieldName);

        foreach (var item in union.DerivedStructs)
            builder.AppendJsonDerivedType($"{item.TagValue}_{union.Name}".Pascalize(), item.TagValue);

        builder
            .AppendTypeDefine(unionTypeName, true)
            .Append('(')
            .AppendConstructorArgumentList(unionFields, baseFields)
            .Append(')');

        if (baseTypeName is { Length: not 0 })
        {
            builder
                .AppendBaseType(baseTypeName)
                .Append('(');
            if (baseFields is not null)
                builder.AppendBaseConstructorArgumentList(baseFields);
            builder.Append(')');
        }

        builder.Append(';');

        yield return (unionTypeName, builder.ToString());

        foreach (var item in union.DerivedStructs
            .SelectMany(i => ParseType($"{i.TagValue}_{union.Name}".Pascalize(), i, unionTypeName, unionFields)))
            yield return item;
    }

    private static IEnumerable<(string TypeName, string TypeCode)> ParseWithDataUnion(
        string unionTypeName,
        AdvancedUnionType union,
        string? baseTypeName = null,
        IEnumerable<Field>? baseFields = null)
    {
        var unionFields = union.BaseFields.ConcatBase(baseFields ?? []).SortFields();

        StringBuilder builder = new();
        builder
            .AppendSummary(union)
            .AppendParams(unionFields)
            .AppendJsonPolymorphic(union.TagFieldName);

        foreach (var item in union.DerivedTypes)
        {
            var typeName = item is RefDerivedType refDerivedType
                ? refDerivedType.RefStructName
                : $"{item.TagValue}_{union.Name}";

            builder.AppendJsonDerivedType(typeName.Pascalize(), item.TagValue);
        }

        builder
            .AppendTypeDefine(unionTypeName, true)
            .Append('(')
            .AppendConstructorArgumentList(unionFields, baseFields)
            .Append(')');

        if (baseTypeName is { Length: not 0 })
        {
            builder
                .AppendBaseType(baseTypeName)
                .Append('(');
            if (baseFields is not null)
                builder.AppendBaseConstructorArgumentList(baseFields);
            builder.Append(')');
        }

        builder.Append(';');

        yield return (unionTypeName, builder.ToString());
        foreach (var field in ParseEnums(unionFields))
            yield return field;

        foreach (var item in union.DerivedTypes
            .SelectMany(i => ParseType($"{i.TagValue}_{union.Name}".Pascalize(), i, unionTypeName, unionFields)))
            yield return item;
    }

    private static IEnumerable<(string TypeName, string TypeCode)> ParseEnums(IEnumerable<Field> fields)
    {
        foreach (var item in fields.OfType<EnumField>())
        {
            var values = item.Values.Select(static i => $"""
                    [global::System.Text.Json.Serialization.JsonStringEnumMemberName("{i}")]
                    {i.Pascalize()},
                """);

            var typeName = item.Name.Pascalize();

            yield return (typeName, $$"""
                /// <summary>
                /// {{item.Description}}
                /// </summary>
                [global::System.Text.Json.Serialization.JsonConverter(typeof(global::System.Text.Json.Serialization.JsonStringEnumConverter<{{typeName}}>))]
                public enum {{typeName}}
                {
                {{string.Join("\r\n", values)}}
                }
                """);
        }
    }

    private static IEnumerable<(string TypeName, string TypeCode)> ParseApi(Api api)
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

    private static IEnumerable<(string TypeName, string TypeCode)> ParseApiCategories(IEnumerable<ApiCategory> categories)
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

    private static IEnumerable<Field> SortFields(this IEnumerable<Field> fields)
        => fields
            .Where(i => !i.DefaultValue.HasValue)
            .Concat(fields.Where(i => i.DefaultValue.HasValue));

    extension(StringBuilder builder)
    {
        public StringBuilder AppendJsonDerivedType(string typeName, string propertyValue)
            => builder.AppendLine($"[global::System.Text.Json.Serialization.JsonDerivedType(typeof({typeName}), \"{propertyValue}\")]");

        public StringBuilder AppendJsonPolymorphic(string propertyName)
            => builder.AppendLine($"[global::System.Text.Json.Serialization.JsonPolymorphic(TypeDiscriminatorPropertyName = \"{propertyName}\")]");

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
