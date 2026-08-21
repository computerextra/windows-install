namespace ComputerExtra.WindowsInstall.Core.Persistence;

public static class ResumeLaunchCommand
{
    public static string Create(string executablePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);

        return $"\"{executablePath.Replace("\"", "\\\"")}\" --resume";
    }
}
