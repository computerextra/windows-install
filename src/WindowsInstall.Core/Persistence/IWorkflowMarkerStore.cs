using ComputerExtra.WindowsInstall.Core.Markers;

namespace ComputerExtra.WindowsInstall.Core.Persistence;

public interface IWorkflowMarkerStore
{
    ValueTask<IReadOnlyCollection<WorkflowMarker>> LoadAsync(
        CancellationToken cancellationToken = default);

    ValueTask SaveAsync(
        IReadOnlyCollection<WorkflowMarker> markers,
        CancellationToken cancellationToken = default);
}