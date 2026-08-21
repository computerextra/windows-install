namespace ComputerExtra.WindowsInstall.Core.State;

public sealed class ResumeApplicationStartup(
    SetupResumeCoordinator resumeCoordinator)
{
    public async ValueTask<SetupRunState> ResumeAsync(
        CancellationToken cancellationToken = default)
    {
        var state = await resumeCoordinator.ResumeAsync(cancellationToken);

        if (state.CurrentStep is null)
        {
            throw new InvalidOperationException(
                "Resume-State enthält keinen fortzusetzenden Workflow-Schritt.");
        }

        return state;
    }
}
