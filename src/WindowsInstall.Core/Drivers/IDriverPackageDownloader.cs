namespace ComputerExtra.WindowsInstall.Core.Drivers;

public interface IDriverPackageDownloader
{
    ValueTask<DriverDownloadResult> DownloadAsync(
        Uri sourceUri,
        string destinationDirectory,
        CancellationToken cancellationToken = default);
}
