namespace ComputerExtra.WindowsInstall.Core.Markers;

public static class WorkflowCompletionResolver
{
    public static bool IsCompleted(
        WorkflowMarkerStatus markerStatus,
        string workflowId,
        RealSystemStateEvidence realSystemState)
    {
        ArgumentNullException.ThrowIfNull(markerStatus);
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowId);

        return realSystemState switch
        {
            RealSystemStateEvidence.ConfirmsCompleted => true,
            RealSystemStateEvidence.ContradictsCompleted => false,
            RealSystemStateEvidence.NotReliablyVerifiable =>
                markerStatus.IsCompleted(workflowId),
            _ => throw new ArgumentOutOfRangeException(
                nameof(realSystemState),
                realSystemState,
                "Unbekannte Bewertung des realen Systemzustands.")
        };
    }
}
