using ComputerExtra.WindowsInstall.Core.Markers;

namespace ComputerExtra.WindowsInstall.Core.Tests.Markers;

[TestClass]
public sealed class MarkerFileDefinitionTests
{
    [TestMethod]
    public void MarkerFile_PathUsesDriveRootAndConfiguredFileName()
    {
        var fullPath = MarkerFileDefinition.FullPath;
        var pathRoot = Path.GetPathRoot(fullPath);
        var directory = Path.GetDirectoryName(fullPath);
        var fileName = Path.GetFileName(fullPath);

        Assert.IsNotNull(pathRoot);
        Assert.IsNotNull(directory);
        Assert.AreEqual(pathRoot, directory);
        Assert.AreEqual(MarkerFileDefinition.FileName, fileName);
    }

    [TestMethod]
    public void MarkerFile_NameIsProjectSpecificJsonFile()
    {
        var fileName = Path.GetFileName(MarkerFileDefinition.FullPath);

        StringAssert.StartsWith(
            fileName,
            "ComputerExtra.WindowsInstall.");

        StringAssert.EndsWith(
            fileName,
            ".json");
    }
}