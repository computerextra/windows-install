using ComputerExtra.WindowsInstall.Core.Discovery;
using ComputerExtra.WindowsInstall.Windows.Discovery;

ISystemIdentityReader reader = new PowerShellCimSystemIdentityReader();
var identity = await reader.ReadAsync();

Console.WriteLine($"ComputerName={identity.ComputerName}");
Console.WriteLine($"Manufacturer={identity.Manufacturer ?? "<null>"}");
Console.WriteLine($"Model={identity.Model ?? "<null>"}");
Console.WriteLine($"DeviceSerialNumber={identity.DeviceSerialNumber ?? "<null>"}");
Console.WriteLine($"DeviceClass={identity.DeviceClass}");
Console.WriteLine($"ManufacturerId={ManufacturerResolver.Resolve(identity.Manufacturer)}");
