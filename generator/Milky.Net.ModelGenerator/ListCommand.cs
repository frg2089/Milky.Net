using DotMake.CommandLine;

using Humanizer;

namespace Milky.Net.ModelGenerator;

[CliCommand(Parent = typeof(RootCommand))]
internal sealed class ListCommand
{
    [CliOption(Required = true)]
    public required FileInfo Input { get; set; }

    public async Task RunAsync()
    {
        if (!Input.Exists)
            throw new FileNotFoundException("Input file not found", Input.FullName);

        var types = await RootCommand.GetDataAsync(Input);
        foreach (var name in types.Keys)
            Console.WriteLine(name.Pascalize());
    }
}
