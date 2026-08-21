using ComputerExtra.WindowsInstall.Core.Persistence;
using ComputerExtra.WindowsInstall.Core.State;

namespace ComputerExtra.WindowsInstall.Core.Tests.Persistence;

[TestClass]
public sealed class JsonFileSetupStateStoreTests
{
    [TestMethod]
    public async Task SaveAndLoadAsync_PreservesResumeState()
    {
        var path = CreateStatePath();

        try
        {
            var state = new SetupRunState();
            state.BeginStep(WorkflowStepId.SystemDetection);
            state.MarkCurrentStepCompleted();
            state.BeginStep(WorkflowStepId.DriverInstallation);
            state.RequestReboot();

            var store = new JsonFileSetupStateStore(path);

            await store.SaveAsync(state);
            var loaded = await store.LoadAsync();

            Assert.IsNotNull(loaded);
            Assert.AreEqual(
                WorkflowStepId.DriverInstallation,
                loaded.CurrentStep);
            Assert.IsTrue(loaded.PendingReboot);
            Assert.IsFalse(loaded.IsCompleted);
            Assert.IsTrue(
                loaded.IsStepCompleted(WorkflowStepId.SystemDetection));
        }
        finally
        {
            DeleteTestDirectory(path);
        }
    }

    [TestMethod]
    public async Task LoadAsync_ReturnsNullWhenStateDoesNotExist()
    {
        var path = CreateStatePath();

        try
        {
            var store = new JsonFileSetupStateStore(path);

            Assert.IsNull(await store.LoadAsync());
        }
        finally
        {
            DeleteTestDirectory(path);
        }
    }

    [TestMethod]
    public async Task DeleteAsync_RemovesPersistedState()
    {
        var path = CreateStatePath();

        try
        {
            var store = new JsonFileSetupStateStore(path);
            var state = new SetupRunState();
            state.BeginStep(WorkflowStepId.DriverInstallation);
            state.RequestReboot();

            await store.SaveAsync(state);
            await store.DeleteAsync();

            Assert.IsFalse(File.Exists(path));
        }
        finally
        {
            DeleteTestDirectory(path);
        }
    }

    [TestMethod]
    public async Task LoadAsync_RejectsUnsupportedSchema()
    {
        var path = CreateStatePath();

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(
                path,
                """
                {
                  "SchemaVersion": 999,
                  "CurrentStep": 20,
                  "PendingReboot": true,
                  "IsCompleted": false,
                  "CompletedSteps": []
                }
                """);

            var store = new JsonFileSetupStateStore(path);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await store.LoadAsync());
        }
        finally
        {
            DeleteTestDirectory(path);
        }
    }

    [TestMethod]
    public async Task LoadAsync_RejectsPendingRebootWithoutCurrentStep()
    {
        var path = CreateStatePath();

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(
                path,
                """
                {
                  "SchemaVersion": 1,
                  "CurrentStep": null,
                  "PendingReboot": true,
                  "IsCompleted": false,
                  "CompletedSteps": []
                }
                """);

            var store = new JsonFileSetupStateStore(path);

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await store.LoadAsync());
        }
        finally
        {
            DeleteTestDirectory(path);
        }
    }

    private static string CreateStatePath()
    {
        return Path.Combine(
            Path.GetTempPath(),
            "ComputerExtra.WindowsInstall.Tests",
            Guid.NewGuid().ToString("N"),
            "resume-state.json");
    }

    private static void DeleteTestDirectory(string statePath)
    {
        var directory = Path.GetDirectoryName(statePath);

        if (
            directory is not null &&
            Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }
}
