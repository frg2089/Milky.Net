[CmdletBinding()]
param (
    [Parameter()]
    [string]
    $Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$Private:ProjectRoot = Join-Path $PSScriptRoot '..'
$Private:ArtifactsPath = Join-Path $Private:ProjectRoot 'artifacts'

Write-Host 'Restoring...' -ForegroundColor Blue
dotnet restore --graph --artifacts-path $Private:ArtifactsPath
if ($LASTEXITCODE -ne 0) {
  throw 'Restore failed.'
}

Write-Host 'Generating...' -ForegroundColor Blue
. $Private:ProjectRoot/scripts/Init-Project.ps1

Write-Host 'Building...' -ForegroundColor Blue
dotnet build --graph --artifacts-path $Private:ArtifactsPath --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
  throw 'Build failed.'
}

Write-Host 'Packing...' -ForegroundColor Blue
dotnet pack --graph --artifacts-path $Private:ArtifactsPath --configuration $Configuration --no-restore --no-build --include-symbols
if ($LASTEXITCODE -ne 0) {
  throw 'Pack failed.'
}