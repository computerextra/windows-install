using ComputerExtra.WindowsInstall.Core.Discovery;

namespace ComputerExtra.WindowsInstall.Core.Tests.Discovery;

[TestClass]
public sealed class SoftwareSelectionResolverTests
{
    [TestMethod]
    public void Resolve_FirstRunSelectsConfiguredDefaults()
    {
        var snapshot = CreateSnapshot();

        var selections = SoftwareSelectionResolver.Resolve(snapshot);

        Assert.IsTrue(Get(selections, InstalledSoftwareId.AdobeReader).IsSelectedByDefault);
        Assert.IsTrue(Get(selections, InstalledSoftwareId.GoogleChrome).IsSelectedByDefault);
        Assert.IsTrue(Get(selections, InstalledSoftwareId.SevenZip).IsSelectedByDefault);

        Assert.IsFalse(Get(selections, InstalledSoftwareId.MozillaFirefox).IsSelectedByDefault);
        Assert.IsFalse(Get(selections, InstalledSoftwareId.MozillaThunderbird).IsSelectedByDefault);
        Assert.IsFalse(Get(selections, InstalledSoftwareId.Microsoft365).IsSelectedByDefault);
    }

    [TestMethod]
    public void Resolve_InstalledOptionalSoftwareIsAutomaticallySelected()
    {
        var snapshot = CreateSnapshot(
            InstalledSoftwareId.MozillaFirefox,
            InstalledSoftwareId.MozillaThunderbird);

        var selections = SoftwareSelectionResolver.Resolve(snapshot);

        var firefox = Get(selections, InstalledSoftwareId.MozillaFirefox);
        var thunderbird = Get(selections, InstalledSoftwareId.MozillaThunderbird);

        Assert.IsTrue(firefox.IsInstalled);
        Assert.IsTrue(firefox.IsSelectedByDefault);
        Assert.IsTrue(thunderbird.IsInstalled);
        Assert.IsTrue(thunderbird.IsSelectedByDefault);
    }

    [TestMethod]
    public void Resolve_InstalledStateDoesNotDependOnDefaultSelection()
    {
        var snapshot = CreateSnapshot();

        var selections = SoftwareSelectionResolver.Resolve(snapshot);

        var chrome = Get(selections, InstalledSoftwareId.GoogleChrome);

        Assert.IsFalse(chrome.IsInstalled);
        Assert.IsTrue(chrome.IsSelectedByDefault);
    }

    [TestMethod]
    public void Resolve_ReturnsEverySupportedSoftwareExactlyOnce()
    {
        var selections = SoftwareSelectionResolver.Resolve(CreateSnapshot());

        Assert.HasCount(
            Enum.GetValues<InstalledSoftwareId>().Length,
            selections);

        Assert.AreEqual(
            selections.Count,
            selections.Select(x => x.SoftwareId).Distinct().Count());
    }

    private static SetupSnapshot CreateSnapshot(
        params InstalledSoftwareId[] installedSoftware)
    {
        return new SetupSnapshot(
            SystemIdentity.Create(
                "TEST-PC",
                "WORTMANN AG",
                "TERRA",
                "ABC123"),
            null,
            installedSoftware);
    }

    private static SoftwareSelectionState Get(
        IReadOnlyList<SoftwareSelectionState> selections,
        InstalledSoftwareId softwareId)
    {
        return selections.Single(x => x.SoftwareId == softwareId);
    }
}