using ComputerExtra.WindowsInstall.Core.Persistence;

namespace ComputerExtra.WindowsInstall.Core.State;

public sealed class SetupRunCompletionCoordinator(
    ISetupStateStore stateStore,
    IResumeRegistration resumeRegistration,
    RuntimeSelfCleanupService runtimeSelfCleanupService)
{
    public async ValueTask CompleteAsync(
        SetupRunState state,
        int currentProcessId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (state.IsCompleted)
        {
            throw new InvalidOperationException(
                "Der WindowsInstall-Workflow ist bereits abgeschlossen.");
        }

        state.MarkRunCompleted();

        await stateStore.SaveAsync(state, cancellationToken);
        await resumeRegistration.DeleteAsync(cancellationToken);
        await runtimeSelfCleanupService.PrepareAfterSuccessfulRunAsync(
            currentProcessId,
            cancellationToken);
    }
}
