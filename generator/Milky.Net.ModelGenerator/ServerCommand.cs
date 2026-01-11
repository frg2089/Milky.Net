using System.Text.Json;

using DotMake.CommandLine;

using Milky.Net.ModelGenerator.Models;
using Milky.Net.SourceGenerator.Common;

namespace Milky.Net.ModelGenerator;

[CliCommand(Parent = typeof(GenerateCommand))]
internal sealed class ServerCommand
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

        foreach (var item in MilkyCSharpApiTypeGenerator.MakeServerApiEndpoints(milkyIR.ApiCategories))
            await File.WriteAllTextAsync(
                Path.Combine(Output.FullName, $"{item.TypeName}.g.cs"),
                item.Code);
    }
}