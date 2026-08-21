namespace ComputerExtra.WindowsInstall.Core.Discovery;

public sealed record SystemIdentity(
    string ComputerName,
    string? Manufacturer,
    string? Model,
    string? DeviceSerialNumber,
    DeviceClass DeviceClass)
{
    public static SystemIdentity Create(
        string computerName,
        string? manufacturer,
        string? model,
        string? deviceSerialNumber,
        DeviceClass deviceClass = DeviceClass.Unknown)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(computerName);

        return new SystemIdentity(
            computerName.Trim(),
            NormalizeOptional(manufacturer),
            NormalizeOptional(model),
            NormalizeOptional(deviceSerialNumber),
            deviceClass);
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}
