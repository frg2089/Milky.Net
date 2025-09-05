using System.Text.Json;

using Humanizer;

using Microsoft.CodeAnalysis;

using Milky.Net.SourceGenerator.Common;

namespace Milky.Net.Server.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class ServerSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(
            context.AdditionalTextsProvider.Where(i => Path.GetFileName(i.Path) is "ApiEndpoints.json"),
            (context, data) =>
            {
                var json = data.GetText()?.ToString();
                if (string.IsNullOrEmpty(json))
                    return;

                var map = JsonSerializer.Deserialize<Dictionary<string, ApiEndPoints>>(json!);
                if (map is null)
                    return;

                List<string> registryConstructorArguments = [];
                List<string> endpointNames = [];
                using StringWriter registryConstructor = new()
                {
                    NewLine = "\r\n" + new string(' ', 4 * 2)
                };
                using StringWriter registryFields = new()
                {
                    NewLine = "\r\n" + new string(' ', 4 * 1)
                };
                using StringWriter registrySwitchCases = new()
                {
                    NewLine = "\r\n" + new string(' ', 4 * 3)
                };
                foreach (var (category, endpoints) in map)
                {
                    var pascalCategory = category.Pascalize();
                    var camelCategory = pascalCategory.Camelize();
                    var fieldName = $"_{camelCategory}";
                    var typeName = $"IMilky{pascalCategory}ApiEndpoints";

                    registryFields.WriteLine($"private readonly {typeName} {fieldName};");
                    registryConstructorArguments.Add($"{typeName} {camelCategory}");
                    registryConstructor.WriteLine($"{fieldName} = {camelCategory};");

                    using StringWriter sw = new()
                    {
                        NewLine = "\r\n" + new string(' ', 4 * 1)
                    };
                    foreach (var endpoint in endpoints.Apis)
                    {
                        var parameter = !string.IsNullOrEmpty(endpoint.InputStruct)
                            ? $"{endpoint.InputStruct} input, "
                            : string.Empty;
                        var returnType = !string.IsNullOrEmpty(endpoint.OutputStruct)
                            ? $"Task<{endpoint.OutputStruct}>"
                            : "Task";

                        sw.WriteLine(
                        $$"""

                            /// <summary>
                            /// {{endpoint.Description}} <br />
                            /// 详见文档 <see href="https://milky.ntqqrev.org/api/{{category}}#{{endpoint.Endpoint}}"/>
                            /// </summary>
                            [ApiEndpoint("{{endpoint.Endpoint}}")]
                            {{returnType}} {{endpoint.Endpoint.Pascalize()}}Async({{parameter}}CancellationToken cancellationToken = default);
                            """);

                        endpointNames.Add(endpoint.Endpoint);
                        registrySwitchCases.WriteLine($"case \"{endpoint.Endpoint}\":");
                        registrySwitchCases.NewLine = "\r\n" + new string(' ', 4 * 4);
                        registrySwitchCases.WriteLine("{");
                        if (!string.IsNullOrEmpty(endpoint.InputStruct))
                        {
                            registrySwitchCases.WriteLine($"Debug.Assert(json is not null);");
                            registrySwitchCases.WriteLine($"var input = json.Value.Deserialize(MilkyJsonSerializerContext.Default.{endpoint.InputStruct});");
                            registrySwitchCases.WriteLine($"Debug.Assert(input is not null);");
                        }
                        if (!string.IsNullOrEmpty(endpoint.OutputStruct))
                            registrySwitchCases.Write($"var output = ");
                        registrySwitchCases.Write($"await {fieldName}.{endpoint.Endpoint.Pascalize()}Async(");
                        if (!string.IsNullOrEmpty(endpoint.InputStruct))
                            registrySwitchCases.Write("input, ");
                        registrySwitchCases.WriteLine("cancellationToken);");
                        registrySwitchCases.NewLine = "\r\n" + new string(' ', 4 * 3);
                        if (!string.IsNullOrEmpty(endpoint.OutputStruct))
                            registrySwitchCases.WriteLine($"return (output, MilkyJsonSerializerContext.Default.{endpoint.OutputStruct});");
                        else
                            registrySwitchCases.WriteLine($"return null;");
                        registrySwitchCases.WriteLine("}");
                    }

                    context.AddSource(
                        $"{typeName}.g.cs",
                        $$"""
                        using Milky.Net.Model;

                        #nullable enable

                        namespace Milky.Net.Server;
                        
                        /// <summary>
                        /// {{endpoints.Name}}
                        /// </summary>
                        public interface {{typeName}}
                        {{{sw}}
                        }
                        """);
                }
                context.AddSource(
                    "MilkyApiEndpoints.g.cs",
                    $$"""
                    using System.Diagnostics;
                    using System.Text.Json;
                    using System.Text.Json.Serialization.Metadata;
                    
                    using Milky.Net.Model;
                    
                    #nullable enable
                    
                    namespace Milky.Net.Server;

                    /// <summary>
                    /// Milky API 端点
                    /// </summary>
                    public partial class MilkyApiEndpoints
                    {
                        {{registryFields}}

                        /// <summary>
                        /// 创建 Milky API 端点处理器
                        /// </summary>
                        public MilkyApiEndpoints({{string.Join(", ", registryConstructorArguments)}})
                        {
                            {{registryConstructor}}
                        }
                    
                        /// <summary>
                        /// 尝试匹配 API 端点
                        /// </summary>
                        public bool CanInvoke(string endpoint) => endpoint switch
                        {
                            {{string.Join(" or ", endpointNames.Select(i => $"\"{i}\""))}} => true,
                            _ => false,
                        };

                        /// <summary>
                        /// 调度 API 请求
                        /// </summary>
                        public async Task<(object Result, JsonTypeInfo Type)?> InvokeAsync(string endpoint, JsonElement? json, CancellationToken cancellationToken = default)
                        {
                            switch(endpoint)
                            {
                                {{registrySwitchCases}}
                            }
                            return null;
                        }
                    }
                    """);
            });
    }
}
