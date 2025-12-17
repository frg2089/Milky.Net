using System.Text.Json;
using System.Xml.Linq;

using Humanizer;

using Microsoft.CodeAnalysis;

using Milky.Net.SourceGenerator.Common;

namespace Milky.Net.Client.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class ApiEndpointsClientGenerator : IIncrementalGenerator
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

                using StringWriter constructor = new()
                {
                    NewLine = "\r\n" + new string(' ', 4 * 2)
                };
                using StringWriter properties = new()
                {
                    NewLine = "\r\n" + new string(' ', 4 * 1)
                };
                foreach (var (category, endpoints) in map)
                {
                    var name = category.Pascalize();
                    var className = $"Milky{name}Client";
                    constructor.WriteLine($"{name} = new(this);");
                    properties.WriteLine($$"""

                        /// <inheritdoc cref="{{className}}" />
                        public {{className}} {{name}} { get; }
                        """);

                    using StringWriter sw = new()
                    {
                        NewLine = "\r\n" + new string(' ', 4 * 1)
                    };

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

                        sw.WriteLine($$"""

                            /// <summary>
                            /// {{endpoint.Description}} <br />
                            /// 详见文档 <see href="https://milky.ntqqrev.org/api/{{category}}#{{endpoint.Endpoint}}"/>
                            /// </summary>
                            public async {{returnType}} {{endpoint.Endpoint.Pascalize()}}Async({{parameter}}CancellationToken cancellationToken = default)
                                => await _client.RequestAsync("{{endpoint.Endpoint}}", {{argument}},
                                    MilkyJsonSerializerContext.Default.{{endpoint.InputStruct ?? "Object"}},
                                    MilkyJsonSerializerContext.Default.{{endpoint.OutputStruct ?? "Object"}},
                                    cancellationToken: cancellationToken);
                            """);
                    }

                    context.AddSource(
                        $"Milky{name}Client.g.cs",
                        $$"""
                        using Milky.Net.Model;
                        
                        #nullable enable
                        
                        namespace Milky.Net.Client;
                        
                        /// <summary>
                        /// {{endpoints.Name}}
                        /// </summary>
                        /// <param name="client"></param>
                        public sealed class {{className}}
                        {
                            private readonly MilkyClient _client;

                            internal {{className}}(MilkyClient client)
                                => _client = client;
                            {{sw}}
                        }
                        """);
                }
                context.AddSource(
                    $"MilkyClient.g.cs",
                    $$"""
                    using System.Collections.Immutable;
                    using System.Linq;

                    #nullable enable
                    
                    namespace Milky.Net.Client;
                    
                    public sealed partial class MilkyClient
                    {
                        /// <summary>
                        /// 创建 Milky 客户端
                        /// </summary>
                        /// <param name="client">Http 客户端实例</param>
                        /// <param name="middleware">请求中间件</param>
                        /// <param name="schedulerFactory">事件调度器，默认为<cref="MilkyEventScheduler"/></param>
                        public MilkyClient(HttpClient client, IEnumerable<IMilkyClientMiddleware>? middleware = default, Func<MilkyClient, IMilkyEventScheduler>? schedulerFactory = null)
                        {
                            _client = client;
                            _middleware = middleware?.Reverse().ToImmutableArray() ?? [];
                            Events = schedulerFactory is not null ? schedulerFactory(this) : new MilkyEventScheduler(this);
                            {{constructor}}
                        }
                        {{properties}}
                    }
                    """);
            });
    }
}
