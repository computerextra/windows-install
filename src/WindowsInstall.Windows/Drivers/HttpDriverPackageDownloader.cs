using ComputerExtra.WindowsInstall.Core.Drivers;

namespace ComputerExtra.WindowsInstall.Windows.Drivers;

public sealed class HttpDriverPackageDownloader(
    HttpClient httpClient) : IDriverPackageDownloader
{
    public async ValueTask<DriverDownloadResult> DownloadAsync(
        Uri sourceUri,
        string destinationDirectory,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceUri);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);

        if (sourceUri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidOperationException(
                "Treiberdownloads müssen HTTPS verwenden.");
        }

        Directory.CreateDirectory(destinationDirectory);

        var fileName = Path.GetFileName(sourceUri.AbsolutePath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new InvalidOperationException(
                "Download-URL enthält keinen Dateinamen.");
        }

        var destinationPath = Path.Combine(
            destinationDirectory,
            Uri.UnescapeDataString(fileName));

        using var response = await httpClient.GetAsync(
            sourceUri,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using (var source = await response.Content.ReadAsStreamAsync(
            cancellationToken))
        await using (var destination = new FileStream(
            destinationPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            81920,
            FileOptions.Asynchronous))
        {
            await source.CopyToAsync(destination, cancellationToken);
        }

        var length = new FileInfo(destinationPath).Length;

        if (length <= 0)
        {
            File.Delete(destinationPath);
            throw new InvalidDataException(
                "Treiberdownload ist leer.");
        }

        return new DriverDownloadResult(
            sourceUri,
            destinationPath,
            length);
    }
}
