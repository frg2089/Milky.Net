pnpm -w --filter=@milky.net/schema-generator gen_types
dotnet run `
  --project $PSScriptRoot/../tools/Milky.Net.ModelGenerator/Milky.Net.ModelGenerator.csproj `
  $PSScriptRoot/../tools/schema-generator/out/Types.json `
  $PSScriptRoot/../src/Milky.Net.Model/generated