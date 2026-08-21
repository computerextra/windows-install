using ComputerExtra.WindowsInstall.Core.Discovery;
using ComputerExtra.WindowsInstall.Core.Drivers;
using ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;
using ComputerExtra.WindowsInstall.Windows.Discovery;
using ComputerExtra.WindowsInstall.Windows.Drivers;
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
    Timeout = TimeSpan.FromMinutes(2)
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

if (!args.Contains(
    "--download-probe",
    StringComparer.OrdinalIgnoreCase))
{
    return drivers.Count > 0 ? 0 : 4;
}

var chipset = drivers.SingleOrDefault(
    driver => driver.Category == WortmannDriverCategory.Chipset);

if (chipset is null)
{
    Console.Error.WriteLine("Kein Chipsatztreiber gefunden.");
    return 5;
}

var probeRoot = Path.Combine(
    Path.GetTempPath(),
    $"ComputerExtra.WindowsInstall.DriverProbe.{Guid.NewGuid():N}");

var downloadDirectory = Path.Combine(probeRoot, "download");
var extractDirectory = Path.Combine(probeRoot, "extracted");

try
{
    IDriverPackageDownloader downloader =
        new HttpDriverPackageDownloader(httpClient);
    IDriverArchiveValidator validator =
        new ZipDriverArchiveValidator();
    IDriverArchiveExtractor extractor =
        new ZipDriverArchiveExtractor();

    var download = await downloader.DownloadAsync(
        chipset.Asset.DownloadUri,
        downloadDirectory);

    await validator.ValidateAsync(download.FilePath);

    var extractedPath = await extractor.ExtractAsync(
        download.FilePath,
        extractDirectory);

    var infFiles = Directory.GetFiles(
        extractedPath,
        "*.inf",
        SearchOption.AllDirectories);

    Console.WriteLine($"DownloadFile={download.FilePath}");
    Console.WriteLine($"DownloadLength={download.Length}");
    Console.WriteLine($"ExtractedPath={extractedPath}");
    Console.WriteLine($"InfCount={infFiles.Length}");

    if (infFiles.Length == 0)
    {
        Console.Error.WriteLine(
            "Treiberarchiv enthält keine INF-Dateien.");
        return 6;
    }

    return 0;
}
finally
{
    if (Directory.Exists(probeRoot))
    {
        Directory.Delete(
            probeRoot,
            recursive: true);
    }
}
