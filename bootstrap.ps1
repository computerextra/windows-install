#requires -Version 5.1
[CmdletBinding()]
param(
    [Parameter()]
    [Uri]$AssetUri = 'https://github.com/computerextra/windows-install/releases/latest/download/WindowsInstall-win-x64.exe',

    [Parameter()]
    [Uri]$ChecksumUri = 'https://github.com/computerextra/windows-install/releases/latest/download/WindowsInstall-win-x64.exe.sha256',

    [Parameter()]
    [switch]$NoLaunch
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

if ($env:OS -ne 'Windows_NT') {
    throw 'WindowsInstall kann nur unter Windows ausgeführt werden.'
}

function Receive-File {
    param(
        [Parameter(Mandatory)]
        [Uri]$SourceUri,

        [Parameter(Mandatory)]
        [string]$DestinationPath
    )

    if ($SourceUri.IsFile) {
        Copy-Item -LiteralPath $SourceUri.LocalPath -Destination $DestinationPath
        return
    }

    if ($SourceUri.Scheme -ne 'https') {
        throw "Nicht unterstütztes Downloadprotokoll: $($SourceUri.Scheme)"
    }

    Invoke-WebRequest `
        -UseBasicParsing `
        -Uri $SourceUri `
        -OutFile $DestinationPath `
        -TimeoutSec 120
}

$runtimeDirectory = Join-Path `
    ([System.IO.Path]::GetTempPath()) `
    ('ComputerExtra.WindowsInstall.{0}' -f [Guid]::NewGuid().ToString('N'))

$executablePath = Join-Path $runtimeDirectory 'WindowsInstall.exe'
$checksumPath = Join-Path $runtimeDirectory 'WindowsInstall.exe.sha256'
$previousBundleExtractBaseDirectory = $env:DOTNET_BUNDLE_EXTRACT_BASE_DIR

try {
    New-Item -ItemType Directory -Path $runtimeDirectory -Force | Out-Null

    Receive-File -SourceUri $AssetUri -DestinationPath $executablePath
    Receive-File -SourceUri $ChecksumUri -DestinationPath $checksumPath

    $checksumText = [System.IO.File]::ReadAllText($checksumPath).Trim()
    $expectedHash = ($checksumText -split '\s+')[0].ToUpperInvariant()

    if ($expectedHash -notmatch '^[0-9A-F]{64}$') {
        throw 'Die heruntergeladene SHA-256-Prüfsumme ist ungültig.'
    }

    $actualHash = (Get-FileHash -LiteralPath $executablePath -Algorithm SHA256).Hash.ToUpperInvariant()

    if ($actualHash -ne $expectedHash) {
        throw 'SHA-256-Prüfung des WindowsInstall-Artefakts fehlgeschlagen.'
    }

    if (-not $NoLaunch) {
        $env:DOTNET_BUNDLE_EXTRACT_BASE_DIR = $runtimeDirectory

        $process = Start-Process `
            -FilePath $executablePath `
            -Wait `
            -PassThru

        if ($process.ExitCode -ne 0) {
            throw "WindowsInstall wurde mit Exitcode $($process.ExitCode) beendet."
        }
    }
}
finally {
    $env:DOTNET_BUNDLE_EXTRACT_BASE_DIR = $previousBundleExtractBaseDirectory

    if (Test-Path -LiteralPath $runtimeDirectory) {
        Remove-Item -LiteralPath $runtimeDirectory -Recurse -Force
    }
}
