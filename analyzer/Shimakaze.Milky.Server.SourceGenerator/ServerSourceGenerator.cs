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
                    foreach (var endpoint in endpoints.Apis)
                    {
                        var parameter = !string.IsNullOrEmpty(endpoint.InputStruct)
                            ? $"{endpoint.InputStruct} input, "
                            : string.Empty;
                        var argument = !string.IsNullOrEmpty(endpoint.InputStruct)
                            ? "input"
                            : "MilkyClient.EmptyObject";
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
                    }
                    sw.WriteLine("}");

                    context.AddSource($"IMilky{name}ApiEndpoints.g.cs", sw.ToString());
                }
            });
    }
}
