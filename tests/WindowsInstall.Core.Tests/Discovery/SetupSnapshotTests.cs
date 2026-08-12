using ComputerExtra.WindowsInstall.Core.Discovery;

namespace ComputerExtra.WindowsInstall.Core.Tests.Discovery;

[TestClass]
public sealed class SetupSnapshotTests
{
    [TestMethod]
    public void Constructor_NormalizesOemDeviceNumber()
    {
        var snapshot = new SetupSnapshot(
            SystemIdentity.Create("TEST-PC", null, null, null),
            " 4711 ");

        Assert.AreEqual("4711", snapshot.OemDeviceNumber);
    }

    [TestMethod]
    public void Constructor_AllowsMissingOemDeviceNumber()
    {
        var snapshot = new SetupSnapshot(
            SystemIdentity.Create("TEST-PC", null, null, null),
            " ");

        Assert.IsNull(snapshot.OemDeviceNumber);
    }

    [TestMethod]
    public void IsInstalled_UsesDetectedSystemState()
    {
        var snapshot = new SetupSnapshot(
            SystemIdentity.Create("TEST-PC", null, null, null),
            null,
            [
                InstalledSoftwareId.GoogleChrome,
                InstalledSoftwareId.SevenZip
            ]);

        Assert.IsTrue(snapshot.IsInstalled(InstalledSoftwareId.GoogleChrome));
        Assert.IsTrue(snapshot.IsInstalled(InstalledSoftwareId.SevenZip));
        Assert.IsFalse(snapshot.IsInstalled(InstalledSoftwareId.MozillaFirefox));
    }

    [TestMethod]
    public void Constructor_RemovesDuplicateSoftwareIds()
    {
        var snapshot = new SetupSnapshot(
            SystemIdentity.Create("TEST-PC", null, null, null),
            null,
            [
                InstalledSoftwareId.GoogleChrome,
                InstalledSoftwareId.GoogleChrome
            ]);

        Assert.HasCount(1, snapshot.InstalledSoftware);
    }
}