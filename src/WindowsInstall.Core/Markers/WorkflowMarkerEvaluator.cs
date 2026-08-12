namespace ComputerExtra.WindowsInstall.Core.Markers;

public static class WorkflowMarkerEvaluator
{
    public static MarkerMatchResult Evaluate(
        WorkflowMarker marker,
        MarkerDeviceContext currentDevice)
    {
        ArgumentNullException.ThrowIfNull(marker);
        ArgumentNullException.ThrowIfNull(currentDevice);

        if (marker.SchemaVersion != WorkflowMarker.CurrentSchemaVersion)
        {
            return MarkerMatchResult.UnsupportedSchema;
        }

        if (!string.Equals(
                marker.Manufacturer,
                currentDevice.Manufacturer,
                StringComparison.OrdinalIgnoreCase))
        {
            return MarkerMatchResult.ManufacturerMismatch;
        }

        if (!string.Equals(
                marker.DeviceSerialNumber,
                currentDevice.DeviceSerialNumber,
                StringComparison.OrdinalIgnoreCase))
        {
            return MarkerMatchResult.DeviceSerialNumberMismatch;
        }

        return MarkerMatchResult.Valid;
    }
}