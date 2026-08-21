namespace ComputerExtra.WindowsInstall.Core.State;

public sealed class SetupRunState
{
    public const int CurrentSchemaVersion = 1;

    private readonly HashSet<WorkflowStepId> _completedSteps = [];

    public int SchemaVersion { get; } = CurrentSchemaVersion;

    public WorkflowStepId? CurrentStep { get; private set; }

    public bool PendingReboot { get; private set; }

    public bool IsCompleted { get; private set; }

    public IReadOnlySet<WorkflowStepId> CompletedSteps => _completedSteps;

    public void BeginStep(WorkflowStepId step)
    {
        EnsureRunIsActive();

        if (_completedSteps.Contains(step))
        {
            throw new InvalidOperationException(
                $"Workflow-Schritt '{step}' ist bereits abgeschlossen.");
        }

        CurrentStep = step;
    }

    public void MarkCurrentStepCompleted()
    {
        EnsureRunIsActive();

        if (CurrentStep is null)
        {
            throw new InvalidOperationException(
                "Es ist kein aktiver Workflow-Schritt vorhanden.");
        }

        _completedSteps.Add(CurrentStep.Value);
        CurrentStep = null;
    }

    public void RequestReboot()
    {
        EnsureRunIsActive();

        if (CurrentStep is null)
        {
            throw new InvalidOperationException(
                "Ein Neustart kann nur für einen aktiven Workflow-Schritt angefordert werden.");
        }

        PendingReboot = true;
    }

    public void ClearPendingReboot()
    {
        EnsureRunIsActive();

        if (!PendingReboot)
        {
            throw new InvalidOperationException(
                "Es steht kein Neustart zur Fortsetzung aus.");
        }

        PendingReboot = false;
    }

    public void MarkRunCompleted()
    {
        EnsureRunIsActive();

        if (CurrentStep is not null)
        {
            throw new InvalidOperationException(
                "Der Workflow kann nicht abgeschlossen werden, solange ein Schritt aktiv ist.");
        }

        if (PendingReboot)
        {
            throw new InvalidOperationException(
                "Der Workflow kann nicht abgeschlossen werden, solange ein Neustart aussteht.");
        }

        IsCompleted = true;
    }

    public bool IsStepCompleted(WorkflowStepId step) => _completedSteps.Contains(step);

    private void EnsureRunIsActive()
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException(
                "Ein bereits abgeschlossener Workflow kann nicht weiter verändert werden.");
        }
    }
}
