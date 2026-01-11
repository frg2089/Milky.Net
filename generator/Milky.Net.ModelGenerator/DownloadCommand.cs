using DotMake.CommandLine;

namespace Milky.Net.ModelGenerator;

[CliCommand(Parent = typeof(RootCommand))]
internal sealed class DownloadCommand
{
    [CliOption(Required = true)]
    public required FileInfo Output { get; set; }

    public async Task RunAsync()
    {
        await using var fs = Output.Create();
        using HttpClient client = new();
        await using var ns = await client.GetStreamAsync("https://milky.ntqqrev.org/raw/milky-ir/ir.json");
        await ns.CopyToAsync(fs);
        await fs.FlushAsync();
    }
}
