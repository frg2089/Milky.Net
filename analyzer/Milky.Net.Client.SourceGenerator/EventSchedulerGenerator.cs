using System.Text.Json;

using Humanizer;

using Microsoft.CodeAnalysis;

using Milky.Net.SourceGenerator.Common;

namespace Milky.Net.Client.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class EventSchedulerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterSourceOutput(
            context.AdditionalTextsProvider.Where(i => Path.GetFileName(i.Path) is "MilkyTypes.json"),
            (context, data) =>
            {
                var json = data.GetText()?.ToString();
                if (string.IsNullOrEmpty(json))
                    return;

                var map = JsonSerializer.Deserialize<Dictionary<string, TypeInfoData>>(json!);
                if (map is null)
                    return;

                if (!map.TryGetValue("Event", out var info) || info is not UnionTypeInfoData union)
                    return;

                using StringWriter eventWriter = new();
                using StringWriter switchWriter = new();
                foreach (var type in union.Types.Select(i => (i.Key.Pascalize(), i.Value.Pascalize())))
                {
                    eventWriter.WriteLine($"""
                    
                        /// <inheritdoc cref="{type.Item2}" />
                        public event ReceivedEventHandler<{type.Item2}>? {type.Item1};
                    """);
                    switchWriter.WriteLine($"""
                                case {type.Item2} e:
                                    {type.Item1}?.Invoke(_client, e);
                                    return;
                    """);
                }

                context.AddSource(
                    "MilkyEventScheduler.g.cs",
                    $$"""
                    using Milky.Net.Model;
                    
                    #nullable enable

                    namespace Milky.Net.Client;
                    public sealed partial class MilkyEventScheduler
                    {{{eventWriter}}
                    
                        internal partial void Received(Event @event)
                        {
                            switch (@event)
                            {
                    {{switchWriter}}
                            }
                        }
                    }
                    """);
            });
    }
}
