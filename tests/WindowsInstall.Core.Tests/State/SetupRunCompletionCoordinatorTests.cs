using ComputerExtra.WindowsInstall.Core.Execution;
using ComputerExtra.WindowsInstall.Core.Persistence;
using ComputerExtra.WindowsInstall.Core.Safety;
using ComputerExtra.WindowsInstall.Core.State;

namespace ComputerExtra.WindowsInstall.Core.Tests.State;

[TestClass]
public sealed class SetupRunCompletionCoordinatorTests
{
    [TestMethod]
    public async Task CompleteAsync_MarksRunCompletedAndRemovesRuntimeArtifacts()
    {
        var runtime = CreateRuntime();
        Directory.CreateDirectory(runtime.RootDirectory);

        try
        {
            var stateStore = new RecordingStateStore();
            var registration = new RecordingResumeRegistration();
            var starter = new RecordingDetachedProcessStarter();
            var cleanup = new RuntimeSelfCleanupService(
                stateStore,
                runtime,
                starter,
                new AllowMutationGuard());
            var coordinator = new SetupRunCompletionCoordinator(
                stateStore,
                registration,
                cleanup);
            var state = new SetupRunState();

            await coordinator.CompleteAsync(state, 1234);

            Assert.IsTrue(state.IsCompleted);
            Assert.AreEqual(1, stateStore.SaveCount);
            Assert.AreEqual(1, stateStore.DeleteCount);
            Assert.AreEqual(1, registration.DeleteCount);
            Assert.AreEqual("powershell.exe", starter.FileName);
        }
        finally
        {
            if (Directory.Exists(runtime.RootDirectory))
            {
                Directory.Delete(runtime.RootDirectory, true);
            }
        }
    }

    [TestMethod]
    public async Task CompleteAsync_DoesNotCleanupWhenRunCannotBeCompleted()
    {
        var runtime = CreateRuntime();
        var stateStore = new RecordingStateStore();
        var registration = new RecordingResumeRegistration();
        var starter = new RecordingDetachedProcessStarter();
        var cleanup = new RuntimeSelfCleanupService(
            stateStore,
            runtime,
            starter,
            new AllowMutationGuard());
        var coordinator = new SetupRunCompletionCoordinator(
            stateStore,
            registration,
            cleanup);
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.DriverInstallation);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await coordinator.CompleteAsync(state, 1234));

        Assert.IsFalse(state.IsCompleted);
        Assert.AreEqual(0, stateStore.SaveCount);
        Assert.AreEqual(0, stateStore.DeleteCount);
        Assert.AreEqual(0, registration.DeleteCount);
        Assert.IsNull(starter.FileName);
    }

    [TestMethod]
    public async Task CompleteAsync_DoesNotCleanupWhenPersistingCompletionFails()
    {
        var runtime = CreateRuntime();
        var stateStore = new RecordingStateStore
        {
            ThrowOnSave = true
        };
        var registration = new RecordingResumeRegistration();
        var starter = new RecordingDetachedProcessStarter();
        var cleanup = new RuntimeSelfCleanupService(
            stateStore,
            runtime,
            starter,
            new AllowMutationGuard());
        var coordinator = new SetupRunCompletionCoordinator(
            stateStore,
            registration,
            cleanup);
        var state = new SetupRunState();

        await Assert.ThrowsExactlyAsync<IOException>(
            async () => await coordinator.CompleteAsync(state, 1234));

        Assert.IsTrue(state.IsCompleted);
        Assert.AreEqual(1, stateStore.SaveCount);
        Assert.AreEqual(0, stateStore.DeleteCount);
        Assert.AreEqual(0, registration.DeleteCount);
        Assert.IsNull(starter.FileName);
    }

    [TestMethod]
    public async Task CompleteAsync_DoesNotCleanupWhenResumeTaskDeletionFails()
    {
        var runtime = CreateRuntime();
        var stateStore = new RecordingStateStore();
        var registration = new RecordingResumeRegistration
        {
            ThrowOnDelete = true
        };
        var starter = new RecordingDetachedProcessStarter();
        var cleanup = new RuntimeSelfCleanupService(
            stateStore,
            runtime,
            starter,
            new AllowMutationGuard());
        var coordinator = new SetupRunCompletionCoordinator(
            stateStore,
            registration,
            cleanup);
        var state = new SetupRunState();

        await Assert.ThrowsExactlyAsync<IOException>(
            async () => await coordinator.CompleteAsync(state, 1234));

        Assert.AreEqual(1, stateStore.SaveCount);
        Assert.AreEqual(0, stateStore.DeleteCount);
        Assert.AreEqual(1, registration.DeleteCount);
        Assert.IsNull(starter.FileName);
    }

    private static ResumeRuntimeLayout CreateRuntime()
    {
        return ResumeRuntimeLayout.Create(
            Path.Combine(
                Path.GetTempPath(),
                "ComputerExtra.WindowsInstall.Tests",
                Guid.NewGuid().ToString("N")));
    }

    private sealed class RecordingStateStore : ISetupStateStore
    {
        public int SaveCount { get; private set; }

        public int DeleteCount { get; private set; }

        public bool ThrowOnSave { get; init; }

        public ValueTask<SetupRunState?> LoadAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult<SetupRunState?>(null);
        }

        public ValueTask SaveAsync(
            SetupRunState state,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SaveCount++;

            if (ThrowOnSave)
            {
                throw new IOException("Testfehler beim Speichern.");
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask DeleteAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            DeleteCount++;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class RecordingResumeRegistration : IResumeRegistration
    {
        public int DeleteCount { get; private set; }

        public bool ThrowOnDelete { get; init; }

        public ValueTask RegisterAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.CompletedTask;
        }

        public ValueTask DeleteAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            DeleteCount++;

            if (ThrowOnDelete)
            {
                throw new IOException("Testfehler beim Löschen des Resume-Tasks.");
            }

            return ValueTask.CompletedTask;
        }
    }

    private sealed class RecordingDetachedProcessStarter : IDetachedProcessStarter
    {
        public string? FileName { get; private set; }

        public void Start(
            string fileName,
            IReadOnlyList<string> arguments)
        {
            FileName = fileName;
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
