[CmdletBinding()]
param (
  [Parameter()]
  [uri]
  $IR
)

$ErrorActionPreference = 'Stop'

$Private:RootPath = Split-Path $PSScriptRoot
$Private:MilkyIR = Join-Path $Private:RootPath 'MilkyIR.json'
$Private:MilkyProps = Join-Path $Private:RootPath 'Milky.props'
$Private:ModelGenerator = Join-Path $Private:RootPath 'generator' 'Milky.Net.ModelGenerator'
$Private:ModelTarget = Join-Path $Private:RootPath 'src' 'Milky.Net.Model' 'Generated'
$Private:ClientTarget = Join-Path $Private:RootPath 'src' 'Milky.Net.Client' 'Generated'
$Private:ServerTarget = Join-Path $Private:RootPath 'src' 'Milky.Net.Server' 'Generated'

Write-Host 'Building Generator...' -ForegroundColor Blue
dotnet build $Private:ModelGenerator
if ($LASTEXITCODE -ne 0) {
  throw 'Build Generator failed.'
}

Write-Host 'Downloading MilkyIR...' -ForegroundColor Blue
dotnet run --no-build --no-launch-profile --project $Private:ModelGenerator -- download --ir $IR --output $Private:MilkyIR
if ($LASTEXITCODE -ne 0) {
  throw 'Download MilkyIR failed.'
}

Write-Host 'Generating MSBuild Props...' -ForegroundColor Blue
dotnet run --no-build --no-launch-profile --project $Private:ModelGenerator -- generate props --source $Private:MilkyIR --output $Private:MilkyProps
if ($LASTEXITCODE -ne 0) {
  throw 'Generate MSBuild Props failed.'
}

Write-Host 'Generating Model Codes...' -ForegroundColor Blue
dotnet run --no-build --no-launch-profile --project $Private:ModelGenerator -- generate models --source $Private:MilkyIR --output $Private:ModelTarget
if ($LASTEXITCODE -ne 0) {
  throw 'Generate Model Codes failed.'
}

Write-Host 'Generating Client Codes...' -ForegroundColor Blue
dotnet run --no-build --no-launch-profile --project $Private:ModelGenerator -- generate client --source $Private:MilkyIR --output $Private:ClientTarget
if ($LASTEXITCODE -ne 0) {
  throw 'Generate Client Codes failed.'
}

Write-Host 'Generating Server Codes...' -ForegroundColor Blue
dotnet run --no-build --no-launch-profile --project $Private:ModelGenerator -- generate server --source $Private:MilkyIR --output $Private:ServerTarget
if ($LASTEXITCODE -ne 0) {
  throw 'Generate Server Codes failed.'
}