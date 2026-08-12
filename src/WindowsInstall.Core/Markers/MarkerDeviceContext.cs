namespace ComputerExtra.WindowsInstall.Core.Markers;

public sealed record MarkerDeviceContext(
    string Manufacturer,
    string DeviceSerialNumber)
{
    public static MarkerDeviceContext Create(
        string manufacturer,
        string deviceSerialNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(manufacturer);
        ArgumentException.ThrowIfNullOrWhiteSpace(deviceSerialNumber);

        return new MarkerDeviceContext(
            manufacturer.Trim(),
            deviceSerialNumber.Trim());
    }
}