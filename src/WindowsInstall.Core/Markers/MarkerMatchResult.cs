namespace ComputerExtra.WindowsInstall.Core.Markers;

public enum MarkerMatchResult
{
    Valid = 0,
    UnsupportedSchema = 1,
    ManufacturerMismatch = 2,
    DeviceSerialNumberMismatch = 3
}