namespace ComputerExtra.WindowsInstall.Core.Discovery;

public sealed record SoftwareSelectionState(
    InstalledSoftwareId SoftwareId,
    bool IsInstalled,
    bool IsSelectedByDefault);