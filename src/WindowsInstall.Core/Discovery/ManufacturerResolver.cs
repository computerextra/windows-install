namespace ComputerExtra.WindowsInstall.Core.Discovery;

public static class ManufacturerResolver
{
    public static ManufacturerId Resolve(string? manufacturer)
    {
        if (string.IsNullOrWhiteSpace(manufacturer))
        {
            return ManufacturerId.Unsupported;
        }

        var value = manufacturer.Trim();

        if (value.Contains("WORTMANN", StringComparison.OrdinalIgnoreCase))
        {
            return ManufacturerId.Wortmann;
        }

        if (value.Contains("LENOVO", StringComparison.OrdinalIgnoreCase))
        {
            return ManufacturerId.Lenovo;
        }

        if (value.Contains("ASUSTEK", StringComparison.OrdinalIgnoreCase)
            || value.Equals("ASUS", StringComparison.OrdinalIgnoreCase))
        {
            return ManufacturerId.Asus;
        }

        if (value.Contains("ACER", StringComparison.OrdinalIgnoreCase))
        {
            return ManufacturerId.Acer;
        }

        return ManufacturerId.Unsupported;
    }
}
