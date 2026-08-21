namespace ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

public sealed record WortmannDownloadAsset(Uri DownloadUri)
{
    public string FileName =>
        Uri.UnescapeDataString(
            Path.GetFileName(DownloadUri.AbsolutePath));
}
