namespace ComputerExtra.WindowsInstall.Core.Drivers;

public interface IDriverArchiveExtractor
{
    ValueTask<string> ExtractAsync(
        string archivePath,
        string destinationDirectory,
        CancellationToken cancellationToken = default);
}
