using System.Diagnostics;
using System.Text.Json;
using ComputerExtra.WindowsInstall.Core.Discovery;

namespace ComputerExtra.WindowsInstall.Windows.Discovery;

public sealed class PowerShellCimSystemIdentityReader : ISystemIdentityReader
{
    private const string DiscoveryCommand =
        "$computer = Get-CimInstance -ClassName Win32_ComputerSystem; " +
        "$bios = Get-CimInstance -ClassName Win32_BIOS; " +
        "[pscustomobject]@{" +
        "ComputerName=$computer.Name;" +
        "Manufacturer=$computer.Manufacturer;" +
        "Model=$computer.Model;" +
        "SerialNumber=$bios.SerialNumber;" +
        "PcSystemType=[uint16]$computer.PCSystemType;" +
        "PcSystemTypeEx=[uint16]$computer.PCSystemTypeEx" +
        "} | ConvertTo-Json -Compress";

    public async ValueTask<SystemIdentity> ReadAsync(
        CancellationToken cancellationToken = default)
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException(
                "Windows-Systemerkennung ist nur unter Windows verfügbar.");
        }

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.StartInfo.ArgumentList.Add("-NoProfile");
        process.StartInfo.ArgumentList.Add("-NonInteractive");
        process.StartInfo.ArgumentList.Add("-Command");
        process.StartInfo.ArgumentList.Add(DiscoveryCommand);

        if (!process.Start())
        {
            throw new InvalidOperationException(
                "Windows-Systemerkennung konnte nicht gestartet werden.");
        }

        var standardOutput = process.StandardOutput.ReadToEndAsync(
            cancellationToken);
        var standardError = process.StandardError.ReadToEndAsync(
            cancellationToken);

        await process.WaitForExitAsync(cancellationToken);

        var output = await standardOutput;
        var error = await standardError;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Windows-Systemerkennung fehlgeschlagen: {error.Trim()}");
        }

        var facts = JsonSerializer.Deserialize<WindowsComputerSystemFacts>(
            output,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            })
            ?? throw new InvalidOperationException(
                "Windows-Systemerkennung lieferte keine verwertbaren Daten.");

        return SystemIdentityResolver.Resolve(facts);
    }
}
