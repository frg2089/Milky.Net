$Private:RootPath = Split-Path $PSScriptRoot
$Private:MilkyIR = Join-Path $Private:RootPath 'MilkyIR.json'
$Private:MilkyProps = Join-Path $Private:RootPath 'Milky.props'
$Private:ModelGenerator = Join-Path $Private:RootPath 'generator' 'Milky.Net.ModelGenerator'
$Private:ModelTarget = Join-Path $Private:RootPath 'src' 'Milky.Net.Model' 'Generated'
$Private:ClientTarget = Join-Path $Private:RootPath 'src' 'Milky.Net.Client' 'Generated'
$Private:ServerTarget = Join-Path $Private:RootPath 'src' 'Milky.Net.Server' 'Generated'

dotnet build $Private:ModelGenerator

dotnet run --no-build --no-launch-profile --project $Private:ModelGenerator -- download --output $Private:MilkyIR

dotnet run --no-build --no-launch-profile --project $Private:ModelGenerator -- generate props --source $Private:MilkyIR --output $Private:MilkyProps 
dotnet run --no-build --no-launch-profile --project $Private:ModelGenerator -- generate models --source $Private:MilkyIR --output $Private:ModelTarget
dotnet run --no-build --no-launch-profile --project $Private:ModelGenerator -- generate client --source $Private:MilkyIR --output $Private:ClientTarget
dotnet run --no-build --no-launch-profile --project $Private:ModelGenerator -- generate server --source $Private:MilkyIR --output $Private:ServerTarget