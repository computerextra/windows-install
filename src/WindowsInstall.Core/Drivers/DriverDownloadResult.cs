namespace ComputerExtra.WindowsInstall.Core.Drivers;

public sealed record DriverDownloadResult(
    Uri SourceUri,
    string FilePath,
    long Length);
