using System.Text.Json;

using DotMake.CommandLine;

using Milky.Net.ModelGenerator.Models;
using Milky.Net.SourceGenerator.Common;

namespace Milky.Net.ModelGenerator;

[CliCommand(Parent = typeof(GenerateCommand))]
internal sealed class PropsCommand
{
    [CliOption(Required = true)]
    public required FileInfo Source { get; set; }

    [CliOption(Required = true)]
    public required FileInfo Output { get; set; }

    public async Task RunAsync()
    {
        MilkyIR milkyIR;

        await using (var fs = Source.OpenRead())
        {
            var json = await JsonSerializer.DeserializeAsync(fs, MilkyJsonSerializerContext.Default.JsonElement);
            milkyIR = MilkyIR.ParseFromJson(json);
        }

        await File.WriteAllTextAsync(
            Output.FullName,
            $"""
            <Project>
                <PropertyGroup>
                    <MilkyVersion>{milkyIR.MilkyVersion}</MilkyVersion>
                    <MilkyPackageVersion>{milkyIR.MilkyPackageVersion}</MilkyPackageVersion>
                </PropertyGroup>

                <PropertyGroup>
                    <PackageReleaseNotes>Milky Version: $(MilkyVersion);@saltify/milky-types Version: $(MilkyPackageVersion)</PackageReleaseNotes>
                </PropertyGroup>

                <ItemGroup>
                    <AssemblyMetadata Include="MilkyVersion" Value="$(MilkyVersion)" />
                    <AssemblyMetadata Include="MilkyPackageVersion" Value="$(MilkyPackageVersion)" />
                </ItemGroup>
            </Project>
            """);
    }
}
