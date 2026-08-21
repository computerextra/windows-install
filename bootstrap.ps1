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

function Get-WindowsInstallEnvironment {
    $operatingSystem = Get-CimInstance `
        -ClassName Win32_OperatingSystem `
        -ErrorAction Stop

    $processors = @(
        Get-CimInstance `
            -ClassName Win32_Processor `
            -ErrorAction Stop
    )

    if ($null -eq $operatingSystem) {
        throw 'Windows-Version konnte nicht ermittelt werden.'
    }

    if ($processors.Count -eq 0) {
        throw 'Prozessorarchitektur konnte nicht ermittelt werden.'
    }

    $operatingSystemVersion = $null

    if (-not [Version]::TryParse(
        [string]$operatingSystem.Version,
        [ref]$operatingSystemVersion)) {
        throw "Ungültige Windows-Version erkannt: $($operatingSystem.Version)"
    }

    [PSCustomObject]@{
        Version = $operatingSystemVersion
        ProductType = [uint32]$operatingSystem.ProductType
        ProcessorArchitectures = [uint16[]]@(
            $processors |
                ForEach-Object {
                    [uint16]$_.Architecture
                }
        )
    }
}

function Assert-WindowsInstallEnvironment {
    param(
        [Parameter(Mandatory)]
        [PSCustomObject]$Environment
    )

    $minimumWindows11Version = [Version]'10.0.22000.0'

    if (
        $Environment.ProductType -ne 1 -or
        $Environment.Version -lt $minimumWindows11Version
    ) {
        throw (
            'Nicht unterstütztes Betriebssystem. ' +
            'WindowsInstall benötigt Windows 11 Client. ' +
            "Erkannt: Version $($Environment.Version), " +
            "ProductType $($Environment.ProductType)."
        )
    }

    $unsupportedArchitectures = @(
        $Environment.ProcessorArchitectures |
            Where-Object {
                $_ -ne 9
            }
    )

    if ($unsupportedArchitectures.Count -gt 0) {
        $detectedArchitectures =
            ($Environment.ProcessorArchitectures -join ', ')

        throw (
            'Nicht unterstützte Prozessorarchitektur. ' +
            'WindowsInstall benötigt Windows 11 x64. ' +
            "Erkannte Architekturwerte: $detectedArchitectures."
        )
    }
}

$windowsInstallEnvironment = Get-WindowsInstallEnvironment
Assert-WindowsInstallEnvironment -Environment $windowsInstallEnvironment

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
