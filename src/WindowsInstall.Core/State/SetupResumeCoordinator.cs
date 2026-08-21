using ComputerExtra.WindowsInstall.Core.Persistence;

namespace ComputerExtra.WindowsInstall.Core.State;

public sealed class SetupResumeCoordinator(
    ISetupStateStore stateStore,
    IResumeRegistration resumeRegistration)
{
    public async ValueTask PrepareForRebootAsync(
        SetupRunState state,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (!state.PendingReboot || state.CurrentStep is null)
        {
            throw new InvalidOperationException(
                "Resume kann nur für einen aktiven Workflow-Schritt mit ausstehendem Neustart vorbereitet werden.");
        }

        await stateStore.SaveAsync(state, cancellationToken);
        await resumeRegistration.RegisterAsync(cancellationToken);
    }

    public async ValueTask<SetupRunState> ResumeAsync(
        CancellationToken cancellationToken = default)
    {
        var state = await stateStore.LoadAsync(cancellationToken)
            ?? throw new InvalidOperationException(
                "Es wurde kein gespeicherter WindowsInstall-Resume-State gefunden.");

        if (!state.PendingReboot || state.CurrentStep is null)
        {
            throw new InvalidOperationException(
                "Der gespeicherte WindowsInstall-State enthält keinen gültigen Neustart-Fortsetzungspunkt.");
        }

        state.ClearPendingReboot();

        await stateStore.SaveAsync(state, cancellationToken);
        await resumeRegistration.DeleteAsync(cancellationToken);

        return state;
    }
}
