using System.Text.Json;

using DotMake.CommandLine;

using Milky.Net.ModelGenerator.Models;
using Milky.Net.SourceGenerator.Common;

namespace Milky.Net.ModelGenerator;

[CliCommand(Parent = typeof(GenerateCommand))]
internal sealed class ClientCommand
{
    [CliOption(Required = true)]
    public required FileInfo Source { get; set; }

    [CliOption(Required = true)]
    public required DirectoryInfo Output { get; set; }

    public async Task RunAsync()
    {
        if (Output.Exists)
            Output.Delete(true);
        Output.Create();

        MilkyIR milkyIR;
        await using (var fs = Source.OpenRead())
        {
            var json = await JsonSerializer.DeserializeAsync(fs, MilkyJsonSerializerContext.Default.JsonElement);
            milkyIR = MilkyIR.ParseFromJson(json);
        }

        foreach (var item in MilkyCSharpApiTypeGenerator.MakeClients(milkyIR.ApiCategories))
            await File.WriteAllTextAsync(
                Path.Combine(Output.FullName, $"{item.TypeName}.g.cs"),
                item.Code);

        MilkyCSharpApiTypeGenerator.MakeEventScheduler(
            milkyIR.CommonStructs
                .OfType<AdvancedUnionType>()
                .First(static i => i.Name is "Event"),
            out var code,
            out var interfaceCode);
        await File.WriteAllTextAsync(
            Path.Combine(Output.FullName, "MilkyEventScheduler.g.cs"),
            code);
            
        await File.WriteAllTextAsync(
            Path.Combine(Output.FullName, "IMilkyEventScheduler.g.cs"),
            interfaceCode);
    }
}
