using ComputerExtra.WindowsInstall.Core.State;

namespace ComputerExtra.WindowsInstall.Core.Tests.State;

[TestClass]
public sealed class SetupRunStateTests
{
    [TestMethod]
    public void StepLifecycle_TracksCompletedStep()
    {
        var state = new SetupRunState();

        state.BeginStep(WorkflowStepId.SystemDetection);
        state.MarkCurrentStepCompleted();

        Assert.IsNull(state.CurrentStep);
        Assert.IsTrue(state.IsStepCompleted(WorkflowStepId.SystemDetection));
        Assert.HasCount(1, state.CompletedSteps);
    }

    [TestMethod]
    public void BeginStep_RejectsAlreadyCompletedStep()
    {
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.SystemDetection);
        state.MarkCurrentStepCompleted();

        Assert.ThrowsExactly<InvalidOperationException>(
            () => state.BeginStep(WorkflowStepId.SystemDetection));
    }

    [TestMethod]
    public void RebootLifecycle_CanBeRequestedAndCleared()
    {
        var state = new SetupRunState();

        state.RequestReboot();
        Assert.IsTrue(state.PendingReboot);

        state.ClearPendingReboot();
        Assert.IsFalse(state.PendingReboot);
    }

    [TestMethod]
    public void MarkRunCompleted_RejectsPendingReboot()
    {
        var state = new SetupRunState();
        state.RequestReboot();

        Assert.ThrowsExactly<InvalidOperationException>(
            state.MarkRunCompleted);
    }

    [TestMethod]
    public void MarkRunCompleted_RejectsActiveStep()
    {
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.Finalization);

        Assert.ThrowsExactly<InvalidOperationException>(
            state.MarkRunCompleted);
    }

    [TestMethod]
    public void CompletedRun_IsImmutable()
    {
        var state = new SetupRunState();
        state.MarkRunCompleted();

        Assert.IsTrue(state.IsCompleted);

        Assert.ThrowsExactly<InvalidOperationException>(
            () => state.BeginStep(WorkflowStepId.SystemDetection));

        Assert.ThrowsExactly<InvalidOperationException>(
            state.RequestReboot);
    }
}