namespace ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

public sealed record WortmannDriverPackage(
    WortmannDriverCategory Category,
    WortmannDownloadAsset Asset);
