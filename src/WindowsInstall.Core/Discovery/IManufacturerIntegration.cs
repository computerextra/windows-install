namespace ComputerExtra.WindowsInstall.Core.Discovery;

public interface IManufacturerIntegration
{
    ManufacturerId Manufacturer { get; }

    bool Supports(SystemIdentity identity);
}
