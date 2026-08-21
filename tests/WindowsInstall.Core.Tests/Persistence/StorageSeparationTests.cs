using ComputerExtra.WindowsInstall.Core.Markers;
using ComputerExtra.WindowsInstall.Core.Persistence;

namespace ComputerExtra.WindowsInstall.Core.Tests.Persistence;

[TestClass]
public sealed class StorageSeparationTests
{
    [TestMethod]
    public void PersistentMarkerAndResumeRuntime_AreStructurallySeparated()
    {
        var commonApplicationData = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid().ToString("N"));
        var runtime = ResumeRuntimeLayout.Create(commonApplicationData);

        Assert.AreEqual(
            Path.Combine(
                commonApplicationData,
                "ComputerExtra",
                "WindowsInstall",
                "Resume"),
            runtime.RootDirectory);
        Assert.AreNotEqual(
            MarkerFileDefinition.FullPath,
            runtime.StatePath);
    }
}
