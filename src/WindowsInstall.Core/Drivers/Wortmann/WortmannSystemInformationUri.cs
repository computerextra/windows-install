namespace ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

public static class WortmannSystemInformationUri
{
    public static Uri Create(string serialNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serialNumber);

        var normalized = serialNumber.Trim();

        if (normalized.Any(character => !char.IsLetterOrDigit(character)))
        {
            throw new ArgumentException(
                "Die Wortmann-Seriennummer darf nur Buchstaben und Ziffern enthalten.",
                nameof(serialNumber));
        }

        return new Uri(
            $"https://www.wortmann.de/de-de/systeminformation/{normalized}.aspx",
            UriKind.Absolute);
    }
}
