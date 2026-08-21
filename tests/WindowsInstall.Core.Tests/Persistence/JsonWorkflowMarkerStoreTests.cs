using ComputerExtra.WindowsInstall.Core.Markers;
using ComputerExtra.WindowsInstall.Core.Persistence;
using ComputerExtra.WindowsInstall.Core.Safety;

namespace ComputerExtra.WindowsInstall.Core.Tests.Persistence;

[TestClass]
public sealed class JsonWorkflowMarkerStoreTests
{
    [TestMethod]
    public async Task SaveAndLoadAsync_PreservesMultipleMarkers()
    {
        var path = CreateMarkerPath();

        try
        {
            var markers = new[]
            {
                CreateMarker("wortmann-drivers", "ABC123"),
                CreateMarker("oem-information", "ABC123")
            };

            var store = new JsonWorkflowMarkerStore(
                path,
                new AllowMutationGuard());

            await store.SaveAsync(markers);
            var loaded = await store.LoadAsync();

            Assert.HasCount(2, loaded);
            CollectionAssert.AreEquivalent(
                markers,
                loaded.ToArray());
        }
        finally
        {
            DeleteTestDirectory(path);
        }
    }

    [TestMethod]
    public async Task LoadAsync_ReturnsEmptyCollectionWhenFileDoesNotExist()
    {
        var path = CreateMarkerPath();

        try
        {
            var store = new JsonWorkflowMarkerStore(
                path,
                new AllowMutationGuard());

            Assert.HasCount(0, await store.LoadAsync());
        }
        finally
        {
            DeleteTestDirectory(path);
        }
    }

    [TestMethod]
    public async Task LoadAsync_RejectsUnsupportedDocumentSchema()
    {
        var path = CreateMarkerPath();

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(
                path,
                """
                {
                  "SchemaVersion": 999,
                  "Markers": []
                }
                """);

            var store = new JsonWorkflowMarkerStore(
                path,
                new AllowMutationGuard());

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await store.LoadAsync());
        }
        finally
        {
            DeleteTestDirectory(path);
        }
    }

    [TestMethod]
    public async Task SaveAsync_DevelopmentGuardBlocksPersistentMutation()
    {
        var path = CreateMarkerPath();

        try
        {
            var store = new JsonWorkflowMarkerStore(
                path,
                new DevelopmentSystemMutationGuard());

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await store.SaveAsync(
                    [CreateMarker("wortmann-drivers", "ABC123")]));

            Assert.IsFalse(File.Exists(path));
        }
        finally
        {
            DeleteTestDirectory(path);
        }
    }

    [TestMethod]
    public async Task SaveAsync_MarksFileHiddenOnWindows()
    {
        if (!OperatingSystem.IsWindows())
        {
            Assert.Inconclusive("Hidden-Dateiattribut wird nur unter Windows geprüft.");
        }

        var path = CreateMarkerPath();

        try
        {
            var store = new JsonWorkflowMarkerStore(
                path,
                new AllowMutationGuard());

            await store.SaveAsync(
                [CreateMarker("wortmann-drivers", "ABC123")]);

            Assert.IsTrue(
                File.GetAttributes(path).HasFlag(FileAttributes.Hidden));
        }
        finally
        {
            DeleteTestDirectory(path);
        }
    }

    private static WorkflowMarker CreateMarker(
        string workflowId,
        string serialNumber)
    {
        return WorkflowMarker.CreateCompleted(
            workflowId,
            new DateTimeOffset(2026, 8, 21, 8, 0, 0, TimeSpan.Zero),
            "0.1.0",
            "WORTMANN AG",
            serialNumber);
    }

    private static string CreateMarkerPath()
    {
        return Path.Combine(
            Path.GetTempPath(),
            "ComputerExtra.WindowsInstall.Tests",
            Guid.NewGuid().ToString("N"),
            MarkerFileDefinition.FileName);
    }

    private static void DeleteTestDirectory(string markerPath)
    {
        var directory = Path.GetDirectoryName(markerPath);

        if (
            directory is not null &&
            Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }

    private sealed class AllowMutationGuard : ISystemMutationGuard
    {
        public void EnsureAllowed(string operation)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        }
    }
}
