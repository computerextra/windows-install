using System.Diagnostics;

namespace ComputerExtra.WindowsInstall.Core.Execution;

public sealed class SystemDetachedProcessStarter : IDetachedProcessStarter
{
    public void Start(
        string fileName,
        IReadOnlyList<string> arguments)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentNullException.ThrowIfNull(arguments);

        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException(
                $"Prozess '{fileName}' konnte nicht gestartet werden.");
    }
}
