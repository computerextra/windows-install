using System.IO.Compression;
using ComputerExtra.WindowsInstall.Core.Drivers;

namespace ComputerExtra.WindowsInstall.Windows.Drivers;

public sealed class ZipDriverArchiveValidator : IDriverArchiveValidator
{
    public ValueTask ValidateAsync(
        string archivePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(archivePath);
        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(archivePath))
        {
            throw new FileNotFoundException(
                "Treiberarchiv wurde nicht gefunden.",
                archivePath);
        }

        if (!string.Equals(
            Path.GetExtension(archivePath),
            ".zip",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                "Treiberarchiv ist kein ZIP-Archiv.");
        }

        using var archive = ZipFile.OpenRead(archivePath);

        if (archive.Entries.Count == 0)
        {
            throw new InvalidDataException(
                "Treiberarchiv enthält keine Dateien.");
        }

        var buffer = new byte[1];

        foreach (var entry in archive.Entries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(entry.Name))
            {
                continue;
            }

            using var stream = entry.Open();
            _ = stream.Read(buffer);
        }

        return ValueTask.CompletedTask;
    }
}
