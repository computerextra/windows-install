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
    public void RebootLifecycle_PreservesCurrentStepUntilResume()
    {
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.DriverInstallation);

        state.RequestReboot();

        Assert.IsTrue(state.PendingReboot);
        Assert.AreEqual(WorkflowStepId.DriverInstallation, state.CurrentStep);

        state.ClearPendingReboot();

        Assert.IsFalse(state.PendingReboot);
        Assert.AreEqual(WorkflowStepId.DriverInstallation, state.CurrentStep);
    }

    [TestMethod]
    public void RequestReboot_RejectsMissingActiveStep()
    {
        var state = new SetupRunState();

        Assert.ThrowsExactly<InvalidOperationException>(
            state.RequestReboot);
    }

    [TestMethod]
    public void ClearPendingReboot_RejectsMissingPendingReboot()
    {
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.DriverInstallation);

        Assert.ThrowsExactly<InvalidOperationException>(
            state.ClearPendingReboot);
    }

    [TestMethod]
    public void MarkRunCompleted_RejectsPendingReboot()
    {
        var state = new SetupRunState();
        state.BeginStep(WorkflowStepId.Finalization);
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
