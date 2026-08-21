namespace ComputerExtra.WindowsInstall.Core.Persistence;

public sealed record ResumeRuntimeLayout(
    string RootDirectory,
    string ExecutablePath,
    string StatePath,
    string LogPath)
{
    public static ResumeRuntimeLayout CreateDefault()
    {
        var commonApplicationData = Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData);

        if (string.IsNullOrWhiteSpace(commonApplicationData))
        {
            throw new InvalidOperationException(
                "Windows CommonApplicationData-Pfad konnte nicht ermittelt werden.");
        }

        return Create(commonApplicationData);
    }

    public static ResumeRuntimeLayout Create(string commonApplicationDataPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commonApplicationDataPath);

        var rootDirectory = Path.Combine(
            commonApplicationDataPath,
            "ComputerExtra",
            "WindowsInstall",
            "Resume");

        return new ResumeRuntimeLayout(
            rootDirectory,
            Path.Combine(rootDirectory, "WindowsInstall.exe"),
            Path.Combine(rootDirectory, "resume-state.json"),
            Path.Combine(rootDirectory, "WindowsInstall.log"));
    }
}
