$project = "FanControl.PowerShell"
$output = "releases"
$ilmerge = "$env:userprofile\.nuget\packages\ilmerge\3.0.29\tools\net452\ILMerge.exe"

dotnet publish $project -c release -o $output
 
#"$env:userprofile\.nuget\packages\ilmerge\3.0.29\tools\net452\ILMerge.exe"

$releaseFilename = "FanControll.PowerShell.zip"

# Path to ILMerge executable
$ilmMergePath = "C:\Path\To\ILMerge.exe"

# List of DLLs to merge
$dllList = @(
    "FanControl.PowerShell.dll",
    "Microsoft.ApplicationInsights.dll",
    "Microsoft.Management.Infrastructure.dll",
    "Microsoft.PowerShell.Commands.Diagnostics.dll",
    "Microsoft.PowerShell.Commands.Management.dll",
    "Microsoft.PowerShell.Commands.Utility.dll",
    "Microsoft.PowerShell.ConsoleHost.dll",
    "Microsoft.PowerShell.CoreCLR.Eventing.dll",
    "Microsoft.PowerShell.MarkdownRender.dll",
    "Microsoft.PowerShell.Security.dll",
    "Microsoft.WSMan.Management.dll",
    "Microsoft.WSMan.Runtime.dll",
    "System.Diagnostics.EventLog.dll",
    "System.Management.Automation.dll",
    "YamlDotNet.dll"
)

# Output directory and file name for merged DLL
#$outputDirectory = "C:\Path\To\Output"
#$outputFileName = "Merged.dll"
#$outputPath = Join-Path -Path $outputDirectory -ChildPath $outputFileName

#cd releases

# Target platform (replace 'net7.0' with the desired target platform)
#$targetPlatform = "net7.0"

# Build the ILMerge command
#$ilmMergeCommand = "$ilmerge /out:`"$outputFileName`" /targetplatform:`"$targetPlatform`" /target:library /ndebug"

$packagePath = "package"

if (Test-Path $packagePath) {
    Remove-Item -path $packagePath -recurse
}

if (Test-Path $releaseFilename) {
    Remove-Item -path $releaseFilename
}

New-Item -ItemType Directory -Path $packagePath

$zipInputFiles = New-Object System.Collections.ArrayList
# Add the input DLLs to the command
foreach ($dll in $dllList) {
    #$src = Join-Path "" $dll
    $zipInputFiles.Add("releases\\$dll")

    Copy-Item -Path "releases\\$dll" -Destination $packagePath
}
Compress-Archive $zipInputFiles $releaseFilename
