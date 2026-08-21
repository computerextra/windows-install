using ComputerExtra.WindowsInstall.Core.Persistence;
using ComputerExtra.WindowsInstall.Core.State;

namespace ComputerExtra.WindowsInstall.Core.Tests.State;

[TestClass]
public sealed class SetupResumeCoordinatorTests
{
    [TestMethod]
    public async Task PrepareForRebootAsync_SavesStateAndRegistersResume()
    {
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.DriverInstallation);
        state.RequestReboot();

        var stateStore = new FakeSetupStateStore();
        var registration = new FakeResumeRegistration();
        var coordinator = new SetupResumeCoordinator(stateStore, registration);

        await coordinator.PrepareForRebootAsync(state);

        Assert.AreSame(state, stateStore.SavedState);
        Assert.AreEqual(1, stateStore.SaveCount);
        Assert.AreEqual(1, registration.RegisterCount);
        Assert.AreEqual(0, registration.DeleteCount);
    }

    [TestMethod]
    public async Task PrepareForRebootAsync_RejectsStateWithoutPendingReboot()
    {
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.DriverInstallation);

        var stateStore = new FakeSetupStateStore();
        var registration = new FakeResumeRegistration();
        var coordinator = new SetupResumeCoordinator(stateStore, registration);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await coordinator.PrepareForRebootAsync(state));

        Assert.AreEqual(0, stateStore.SaveCount);
        Assert.AreEqual(0, registration.RegisterCount);
    }

    [TestMethod]
    public async Task ResumeAsync_ContinuesExactStepAndRemovesResumeRegistration()
    {
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.DriverInstallation);
        state.RequestReboot();

        var stateStore = new FakeSetupStateStore
        {
            LoadedState = state
        };
        var registration = new FakeResumeRegistration();
        var coordinator = new SetupResumeCoordinator(stateStore, registration);

        var resumedState = await coordinator.ResumeAsync();

        Assert.AreSame(state, resumedState);
        Assert.IsFalse(resumedState.PendingReboot);
        Assert.AreEqual(
            WorkflowStepId.DriverInstallation,
            resumedState.CurrentStep);
        Assert.AreEqual(1, stateStore.LoadCount);
        Assert.AreEqual(1, stateStore.SaveCount);
        Assert.AreEqual(1, registration.DeleteCount);
    }

    [TestMethod]
    public async Task ResumeAsync_RejectsMissingState()
    {
        var stateStore = new FakeSetupStateStore();
        var registration = new FakeResumeRegistration();
        var coordinator = new SetupResumeCoordinator(stateStore, registration);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await coordinator.ResumeAsync());

        Assert.AreEqual(1, stateStore.LoadCount);
        Assert.AreEqual(0, stateStore.SaveCount);
        Assert.AreEqual(0, registration.DeleteCount);
    }

    [TestMethod]
    public async Task ResumeAsync_RejectsStateWithoutPendingReboot()
    {
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.DriverInstallation);

        var stateStore = new FakeSetupStateStore
        {
            LoadedState = state
        };
        var registration = new FakeResumeRegistration();
        var coordinator = new SetupResumeCoordinator(stateStore, registration);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await coordinator.ResumeAsync());

        Assert.AreEqual(0, stateStore.SaveCount);
        Assert.AreEqual(0, registration.DeleteCount);
    }

    private sealed class FakeSetupStateStore : ISetupStateStore
    {
        public SetupRunState? LoadedState { get; init; }

        public SetupRunState? SavedState { get; private set; }

        public int LoadCount { get; private set; }

        public int SaveCount { get; private set; }

        public int DeleteCount { get; private set; }

        public ValueTask<SetupRunState?> LoadAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LoadCount++;
            return ValueTask.FromResult(LoadedState);
        }

        public ValueTask SaveAsync(
            SetupRunState state,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SavedState = state;
            SaveCount++;
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

    private sealed class FakeResumeRegistration : IResumeRegistration
    {
        public int RegisterCount { get; private set; }

        public int DeleteCount { get; private set; }

        public ValueTask RegisterAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RegisterCount++;
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
}
