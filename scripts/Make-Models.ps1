pnpm -w --filter=@shimakaze.milky/schema-generator gen_types
dotnet run `
  --project $PSScriptRoot/../tools/Shimakaze.Milky.ModelGenerator/Shimakaze.Milky.ModelGenerator.csproj `
  $PSScriptRoot/../tools/schema-generator/out/Types.json `
  $PSScriptRoot/../src/Shimakaze.Milky.Model/generated