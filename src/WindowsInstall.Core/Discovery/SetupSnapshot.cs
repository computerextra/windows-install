namespace ComputerExtra.WindowsInstall.Core.Discovery;

public sealed class SetupSnapshot
{
    private readonly HashSet<InstalledSoftwareId> _installedSoftware;

    public SetupSnapshot(
        SystemIdentity identity,
        string? oemDeviceNumber,
        IEnumerable<InstalledSoftwareId>? installedSoftware = null)
    {
        ArgumentNullException.ThrowIfNull(identity);

        Identity = identity;
        OemDeviceNumber = string.IsNullOrWhiteSpace(oemDeviceNumber)
            ? null
            : oemDeviceNumber.Trim();

        _installedSoftware = installedSoftware is null
            ? []
            : [.. installedSoftware];
    }

    public SystemIdentity Identity { get; }

    public string? OemDeviceNumber { get; }

    public IReadOnlySet<InstalledSoftwareId> InstalledSoftware => _installedSoftware;

    public bool IsInstalled(InstalledSoftwareId softwareId) =>
        _installedSoftware.Contains(softwareId);
}