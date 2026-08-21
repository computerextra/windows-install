using System.IO.Compression;
using ComputerExtra.WindowsInstall.Windows.Drivers;

namespace ComputerExtra.WindowsInstall.Windows.Tests.Drivers;

[TestClass]
public sealed class ZipDriverArchiveValidatorTests
{
    [TestMethod]
    public async Task ValidateAsync_AcceptsReadableZip()
    {
        var root = CreateTempRoot();

        try
        {
            var archivePath = Path.Combine(root, "driver.zip");
            CreateZip(archivePath, "driver.inf", "fixture");

            var validator = new ZipDriverArchiveValidator();

            await validator.ValidateAsync(archivePath);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public async Task ValidateAsync_RejectsCorruptZip()
    {
        var root = CreateTempRoot();

        try
        {
            var archivePath = Path.Combine(root, "driver.zip");
            await File.WriteAllTextAsync(
                archivePath,
                "not-a-zip");

            var validator = new ZipDriverArchiveValidator();

            await Assert.ThrowsExactlyAsync<InvalidDataException>(
                () => validator.ValidateAsync(archivePath).AsTask());
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public async Task ExtractAsync_ExtractsArchive()
    {
        var root = CreateTempRoot();

        try
        {
            var archivePath = Path.Combine(root, "driver.zip");
            var destination = Path.Combine(root, "extracted");
            CreateZip(archivePath, "driver.inf", "fixture");

            var extractor = new ZipDriverArchiveExtractor();

            var result = await extractor.ExtractAsync(
                archivePath,
                destination);

            Assert.AreEqual(destination, result);
            Assert.IsTrue(
                File.Exists(
                    Path.Combine(destination, "driver.inf")));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    private static string CreateTempRoot()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            $"WindowsInstall.DriverArchiveTests.{Guid.NewGuid():N}");

        Directory.CreateDirectory(root);
        return root;
    }

    private static void CreateZip(
        string archivePath,
        string entryName,
        string content)
    {
        using var archive = ZipFile.Open(
            archivePath,
            ZipArchiveMode.Create);

        var entry = archive.CreateEntry(entryName);

        using var writer = new StreamWriter(entry.Open());
        writer.Write(content);
    }
}
