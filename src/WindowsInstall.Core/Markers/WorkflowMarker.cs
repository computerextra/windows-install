namespace ComputerExtra.WindowsInstall.Core.Markers;

public sealed record WorkflowMarker(
    int SchemaVersion,
    string WorkflowId,
    DateTimeOffset CompletedAtUtc,
    string InstallerVersion,
    string Manufacturer,
    string DeviceSerialNumber)
{
    public const int CurrentSchemaVersion = 1;

    public static WorkflowMarker CreateCompleted(
        string workflowId,
        DateTimeOffset completedAtUtc,
        string installerVersion,
        string manufacturer,
        string deviceSerialNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(workflowId);
        ArgumentException.ThrowIfNullOrWhiteSpace(installerVersion);
        ArgumentException.ThrowIfNullOrWhiteSpace(manufacturer);
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceSerialNumber);

        return new WorkflowMarker(
            CurrentSchemaVersion,
            workflowId.Trim(),
            completedAtUtc.ToUniversalTime(),
            installerVersion.Trim(),
            manufacturer.Trim(),
            deviceSerialNumber.Trim());
    }
}