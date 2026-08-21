using ComputerExtra.WindowsInstall.Core.Persistence;
using ComputerExtra.WindowsInstall.Core.State;

namespace ComputerExtra.WindowsInstall.Core.Tests.State;

[TestClass]
public sealed class ResumeApplicationStartupTests
{
    [TestMethod]
    public async Task ResumeAsync_ReturnsExactPersistedWorkflowStep()
    {
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.DriverInstallation);
        state.RequestReboot();

        var stateStore = new FakeStateStore(state);
        var registration = new FakeResumeRegistration();
        var coordinator = new SetupResumeCoordinator(
            stateStore,
            registration);
        var startup = new ResumeApplicationStartup(coordinator);

        var resumedState = await startup.ResumeAsync();

        Assert.AreEqual(
            WorkflowStepId.DriverInstallation,
            resumedState.CurrentStep);
        Assert.IsFalse(resumedState.PendingReboot);
        Assert.AreEqual(1, registration.DeleteCount);
    }

    [TestMethod]
    public async Task ResumeAsync_RejectsMissingPersistedState()
    {
        var coordinator = new SetupResumeCoordinator(
            new FakeStateStore(null),
            new FakeResumeRegistration());
        var startup = new ResumeApplicationStartup(coordinator);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await startup.ResumeAsync());
    }

    private sealed class FakeStateStore(SetupRunState? state) : ISetupStateStore
    {
        public ValueTask<SetupRunState?> LoadAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(state);
        }

        public ValueTask SaveAsync(
            SetupRunState stateToSave,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.CompletedTask;
        }

        public ValueTask DeleteAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.CompletedTask;
        }
    }

    private sealed class FakeResumeRegistration : IResumeRegistration
    {
        public int DeleteCount { get; private set; }

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
            return ValueTask.CompletedTask;
        }
    }
}
