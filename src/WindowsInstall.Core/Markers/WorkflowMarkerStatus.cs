namespace ComputerExtra.WindowsInstall.Core.Markers;

public sealed class WorkflowMarkerStatus
{
    private readonly Dictionary<string, WorkflowMarker> _validMarkers;

    private WorkflowMarkerStatus(
        Dictionary<string, WorkflowMarker> validMarkers)
    {
        _validMarkers = validMarkers;
    }

    public static WorkflowMarkerStatus Create(
        IEnumerable<WorkflowMarker> markers,
        MarkerDeviceContext currentDevice)
    {
        ArgumentNullException.ThrowIfNull(markers);
        ArgumentNullException.ThrowIfNull(currentDevice);

        var validMarkers = new Dictionary<string, WorkflowMarker>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var marker in markers)
        {
            ArgumentNullException.ThrowIfNull(marker);

            if (WorkflowMarkerEvaluator.Evaluate(marker, currentDevice)
                != MarkerMatchResult.Valid)
            {
                continue;
            }

            if (validMarkers.TryGetValue(marker.WorkflowId, out var existing))
            {
                if (marker.CompletedAtUtc > existing.CompletedAtUtc)
                {
                    validMarkers[marker.WorkflowId] = marker;
                }

                continue;
            }

            validMarkers.Add(marker.WorkflowId, marker);
        }

        return new WorkflowMarkerStatus(validMarkers);
    }

    public bool IsCompleted(string workflowId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowId);

        return _validMarkers.ContainsKey(workflowId.Trim());
    }

    public WorkflowMarker? GetCompletedMarker(string workflowId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowId);

        return _validMarkers.GetValueOrDefault(workflowId.Trim());
    }
}
