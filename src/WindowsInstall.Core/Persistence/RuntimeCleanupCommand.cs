namespace ComputerExtra.WindowsInstall.Core.Persistence;

public static class RuntimeCleanupCommand
{
    public static IReadOnlyList<string> CreateArguments(
        string runtimeDirectory,
        int processId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(runtimeDirectory);

        if (processId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(processId));
        }

        var escapedDirectory = runtimeDirectory.Replace("'", "''");

        var command =
            $"$process = Get-Process -Id {processId} -ErrorAction SilentlyContinue; " +
            "if ($null -ne $process) { $process.WaitForExit() }; " +
            $"Remove-Item -LiteralPath '{escapedDirectory}' -Recurse -Force -ErrorAction SilentlyContinue";

        return
        [
            "-NoProfile",
            "-NonInteractive",
            "-WindowStyle", "Hidden",
            "-Command", command
        ];
    }
}
