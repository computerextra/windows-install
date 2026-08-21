namespace ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

public interface IWortmannSystemInformationClient
{
    ValueTask<IReadOnlyList<WortmannDownloadAsset>> GetAssetsAsync(
        string serialNumber,
        CancellationToken cancellationToken = default);
}
