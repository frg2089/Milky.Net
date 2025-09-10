using System.Diagnostics;
using System.Text.Json;

using DotMake.CommandLine;

namespace Milky.Net.ModelGenerator;

[CliCommand]
internal sealed class RootCommand
{
    public static async Task<Dictionary<string, TypeInfoData>> GetDataAsync(FileInfo input)
    {
        await using var fs = input.OpenRead();
        var types = await JsonSerializer.DeserializeAsync<Dictionary<string, TypeInfoData>>(fs);
        Debug.Assert(types is not null);
        return types;
    }
}
