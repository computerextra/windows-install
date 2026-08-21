using ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

namespace ComputerExtra.WindowsInstall.Windows.Drivers.Wortmann;

public sealed class HttpWortmannSystemInformationClient(
    HttpClient httpClient) : IWortmannSystemInformationClient
{
    public async ValueTask<IReadOnlyList<WortmannDownloadAsset>> GetAssetsAsync(
        string serialNumber,
        CancellationToken cancellationToken = default)
    {
        var uri = WortmannSystemInformationUri.Create(serialNumber);

        using var response = await httpClient.GetAsync(
            uri,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync(
            cancellationToken);

        return WortmannWindows11DriverAssetParser.Parse(html);
    }
}
