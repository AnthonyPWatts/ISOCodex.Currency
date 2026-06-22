param(
    [string]$Configuration = "Release",
    [string]$OutputPath,
    [string]$Version,
    [switch]$ContinuousIntegrationBuild
)

$ErrorActionPreference = "Stop"

function Invoke-DotNet {
    $dotnetArguments = $args

    & dotnet @dotnetArguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($dotnetArguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path $repoRoot "artifacts"
}

New-Item -ItemType Directory -Force -Path $OutputPath | Out-Null

$projects = Get-ChildItem (Join-Path $repoRoot "src") -Filter "*.csproj" -Recurse |
    Sort-Object FullName

foreach ($project in $projects) {
    $arguments = @(
        "pack",
        $project.FullName,
        "-c",
        $Configuration,
        "--no-build",
        "-o",
        $OutputPath
    )

    if (-not [string]::IsNullOrWhiteSpace($Version)) {
        $arguments += "/p:Version=$Version"
    }

    if ($ContinuousIntegrationBuild) {
        $arguments += "/p:ContinuousIntegrationBuild=true"
    }

    Invoke-DotNet @arguments
}
