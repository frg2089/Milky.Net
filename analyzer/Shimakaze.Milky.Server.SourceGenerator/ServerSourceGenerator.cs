using System.Diagnostics;
using System.Text.Json;

using Humanizer;

using Microsoft.CodeAnalysis;

using Shimakaze.Milky.SourceGenerator.Common;

namespace Shimakaze.Milky.Server.SourceGenerator;

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

                foreach (var (category, endpoints) in map)
                {
                    var name = char.ToUpperInvariant(category[0]) + category[1..];
                    using StringWriter sw = new();
                    using StringWriter registry = new();

                    sw.WriteLine($$"""
                        using Shimakaze.Milky.Model.Api.{{name}};

                        #nullable enable

                        namespace Shimakaze.Milky.Server;
                        
                        /// <summary>
                        /// {{endpoints.Name}}
                        /// </summary>
                        public interface IMilky{{name}}ApiEndpoints
                        {
                        """);
                    registry.WriteLine($$"""
                        using System.Diagnostics;
                        using System.Text.Json;
                        using System.Text.Json.Serialization.Metadata;

                        using Shimakaze.Milky.Model.Api.{{name}};

                        #nullable enable

                        namespace Shimakaze.Milky.Server;
                        
                        /// <summary>
                        /// {{endpoints.Name}}
                        /// </summary>
                        public static class Milky{{name}}ApiEndpoints
                        {
                            public static async Task<(bool Success, object? Result, JsonTypeInfo? Type)> TryInvokeAsync(
                                this IMilky{{name}}ApiEndpoints endpoints,
                                string api,
                                JsonElement? json,
                                CancellationToken cancellationToken = default)
                            {
                                switch(api)
                                {
                        """);
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

                        registry.WriteLine($"case \"{endpoint.Endpoint}\":");
                        registry.WriteLine("{");
                        if (!string.IsNullOrEmpty(endpoint.InputStruct))
                        {
                            registry.WriteLine($"Debug.Assert(json is not null);");
                            registry.WriteLine($"var input = json.Value.Deserialize(MilkyJsonSerializerContext.Default.{endpoint.InputStruct});");
                            registry.WriteLine($"Debug.Assert(input is not null);");
                        }
                        if (!string.IsNullOrEmpty(endpoint.OutputStruct))
                            registry.Write($"var output = ");
                        registry.Write($"await endpoints.{endpoint.Endpoint.Pascalize()}Async(");
                        if (!string.IsNullOrEmpty(endpoint.InputStruct))
                            registry.Write("input, ");
                        registry.WriteLine("cancellationToken);");
                        if (!string.IsNullOrEmpty(endpoint.OutputStruct))
                            registry.WriteLine($"return (true, output, MilkyJsonSerializerContext.Default.{endpoint.OutputStruct});");
                        else
                            registry.WriteLine($"return (true, null, null);");
                        registry.WriteLine("}");
                    }
                    sw.WriteLine("}");
                    registry.WriteLine("""
                                    default:    
                                        return (false, null, null);
                                }
                            }
                        }
                        """);

                    context.AddSource($"IMilky{name}ApiEndpoints.g.cs", sw.ToString());
                    context.AddSource($"Milky{name}ApiEndpoints.g.cs", registry.ToString());
                }
            });
    }
}
