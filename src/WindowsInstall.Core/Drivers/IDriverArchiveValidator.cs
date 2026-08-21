namespace ComputerExtra.WindowsInstall.Core.Drivers;

public interface IDriverArchiveValidator
{
    ValueTask ValidateAsync(
        string archivePath,
        CancellationToken cancellationToken = default);
}
