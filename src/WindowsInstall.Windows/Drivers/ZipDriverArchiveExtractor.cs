using System.IO.Compression;
using ComputerExtra.WindowsInstall.Core.Drivers;

namespace ComputerExtra.WindowsInstall.Windows.Drivers;

public sealed class ZipDriverArchiveExtractor : IDriverArchiveExtractor
{
    public ValueTask<string> ExtractAsync(
        string archivePath,
        string destinationDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(archivePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);
        cancellationToken.ThrowIfCancellationRequested();

        if (Directory.Exists(destinationDirectory))
        {
            throw new IOException(
                $"Zielverzeichnis existiert bereits: {destinationDirectory}");
        }

        Directory.CreateDirectory(destinationDirectory);

        try
        {
            ZipFile.ExtractToDirectory(
                archivePath,
                destinationDirectory,
                overwriteFiles: false);
        }
        catch
        {
            Directory.Delete(
                destinationDirectory,
                recursive: true);
            throw;
        }

        return ValueTask.FromResult(destinationDirectory);
    }
}
