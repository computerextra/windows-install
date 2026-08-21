using ComputerExtra.WindowsInstall.Core.Safety;

namespace ComputerExtra.WindowsInstall.Core.Persistence;

public sealed class ResumeRuntimeStager(
    ResumeRuntimeLayout runtimeLayout,
    ISystemMutationGuard mutationGuard)
{
    public async ValueTask StageExecutableAsync(
        string sourceExecutablePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceExecutablePath);

        mutationGuard.EnsureAllowed(
            "WindowsInstall-Resume-Runtime bereitstellen");

        if (!File.Exists(sourceExecutablePath))
        {
            throw new FileNotFoundException(
                "WindowsInstall-Ausgangsdatei für Resume wurde nicht gefunden.",
                sourceExecutablePath);
        }

        Directory.CreateDirectory(runtimeLayout.RootDirectory);

        var tempPath = runtimeLayout.ExecutablePath + ".tmp";

        try
        {
            await using (var source = new FileStream(
                sourceExecutablePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                81920,
                FileOptions.Asynchronous | FileOptions.SequentialScan))
            await using (var destination = new FileStream(
                tempPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                81920,
                FileOptions.Asynchronous))
            {
                await source.CopyToAsync(destination, cancellationToken);
                await destination.FlushAsync(cancellationToken);
            }

            File.Move(
                tempPath,
                runtimeLayout.ExecutablePath,
                true);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
}
