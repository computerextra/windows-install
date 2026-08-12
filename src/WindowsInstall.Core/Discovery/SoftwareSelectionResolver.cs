namespace ComputerExtra.WindowsInstall.Core.Discovery;

public static class SoftwareSelectionResolver
{
    private static readonly HashSet<InstalledSoftwareId> FirstRunDefaults =
    [
        InstalledSoftwareId.AdobeReader,
        InstalledSoftwareId.GoogleChrome,
        InstalledSoftwareId.SevenZip
    ];

    public static IReadOnlyList<SoftwareSelectionState> Resolve(
        SetupSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        return Enum.GetValues<InstalledSoftwareId>()
            .Select(softwareId =>
            {
                var isInstalled = snapshot.IsInstalled(softwareId);

                return new SoftwareSelectionState(
                    softwareId,
                    isInstalled,
                    isInstalled || FirstRunDefaults.Contains(softwareId));
            })
            .ToArray();
    }
}