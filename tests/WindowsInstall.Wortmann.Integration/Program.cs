using ComputerExtra.WindowsInstall.Core.Discovery;
using ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;
using ComputerExtra.WindowsInstall.Windows.Discovery;
using ComputerExtra.WindowsInstall.Windows.Drivers.Wortmann;

ISystemIdentityReader identityReader = new PowerShellCimSystemIdentityReader();
var identity = await identityReader.ReadAsync();

if (ManufacturerResolver.Resolve(identity.Manufacturer) != ManufacturerId.Wortmann)
{
    Console.Error.WriteLine(
        $"Kein Wortmann-System erkannt: {identity.Manufacturer ?? "<null>"}");
    return 2;
}

if (string.IsNullOrWhiteSpace(identity.DeviceSerialNumber))
{
    Console.Error.WriteLine("Keine SMBIOS-Seriennummer erkannt.");
    return 3;
}

using var httpClient = new HttpClient
{
    Timeout = TimeSpan.FromSeconds(30)
};

IWortmannSystemInformationClient client =
    new HttpWortmannSystemInformationClient(httpClient);

var assets = await client.GetAssetsAsync(identity.DeviceSerialNumber);
var drivers = WortmannDriverCatalog.SelectForWindows11(assets);

Console.WriteLine($"SerialNumber={identity.DeviceSerialNumber}");
Console.WriteLine($"AssetCount={assets.Count}");
Console.WriteLine($"DriverCount={drivers.Count}");

foreach (var driver in drivers)
{
    Console.WriteLine(
        $"{driver.Category}={driver.Asset.DownloadUri.AbsoluteUri}");
}

return drivers.Count > 0 ? 0 : 4;
