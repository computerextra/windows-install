using System.Text.RegularExpressions;

namespace ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

public static partial class WortmannDriverCatalog
{
    public static IReadOnlyList<WortmannDriverPackage> SelectForWindows11(
        IEnumerable<WortmannDownloadAsset> assets)
    {
        ArgumentNullException.ThrowIfNull(assets);

        return assets
            .Select(asset => TryCreateCandidate(asset))
            .Where(candidate => candidate is not null)
            .Cast<Candidate>()
            .GroupBy(candidate => candidate.Category)
            .Select(group => group
                .OrderByDescending(candidate => candidate.OsPreference)
                .ThenByDescending(candidate => candidate.Version)
                .ThenByDescending(
                    candidate => candidate.Asset.DownloadUri.AbsoluteUri,
                    StringComparer.OrdinalIgnoreCase)
                .First())
            .OrderBy(candidate => candidate.Category)
            .Select(candidate => new WortmannDriverPackage(
                candidate.Category,
                candidate.Asset))
            .ToArray();
    }

    private static Candidate? TryCreateCandidate(
        WortmannDownloadAsset asset)
    {
        ArgumentNullException.ThrowIfNull(asset);

        var path = asset.DownloadUri.AbsolutePath;
        var fileName = asset.FileName;
        var upperPath = path.ToUpperInvariant();
        var upperFileName = fileName.ToUpperInvariant();

        if (!upperFileName.EndsWith(".ZIP", StringComparison.Ordinal))
        {
            return null;
        }

        if (upperPath.Contains("/T_MS_REC_", StringComparison.Ordinal)
            || upperPath.Contains("/PC_TERRA_", StringComparison.Ordinal)
            || upperFileName.Contains("ARMOURYCRATE", StringComparison.Ordinal)
            || upperFileName.Contains("MANUAL", StringComparison.Ordinal)
            || upperFileName.Contains("HANDBUCH", StringComparison.Ordinal)
            || upperFileName.Contains("GUIDE", StringComparison.Ordinal))
        {
            return null;
        }

        var category = ResolveCategory(upperPath);

        if (category is null)
        {
            return null;
        }

        var mentionsWindows10 =
            upperFileName.Contains("WIN10", StringComparison.Ordinal)
            || upperFileName.Contains("W10_", StringComparison.Ordinal);

        var mentionsWindows11 =
            upperFileName.Contains("WIN11", StringComparison.Ordinal)
            || upperFileName.Contains("W11_", StringComparison.Ordinal);

        if (mentionsWindows10 && !mentionsWindows11)
        {
            return null;
        }

        var osPreference = mentionsWindows11 ? 2 : 1;

        return new Candidate(
            category.Value,
            asset,
            osPreference,
            ParseVersion(fileName));
    }

    private static WortmannDriverCategory? ResolveCategory(
        string upperPath)
    {
        if (upperPath.Contains("-CHIP/", StringComparison.Ordinal))
        {
            return WortmannDriverCategory.Chipset;
        }

        if (upperPath.Contains("-DTT/", StringComparison.Ordinal))
        {
            return WortmannDriverCategory.DynamicTuning;
        }

        if (upperPath.Contains("-LAN/", StringComparison.Ordinal))
        {
            return WortmannDriverCategory.Network;
        }

        if (upperPath.Contains("-MEI/", StringComparison.Ordinal))
        {
            return WortmannDriverCategory.ManagementEngine;
        }

        if (upperPath.Contains("-RST/", StringComparison.Ordinal))
        {
            return WortmannDriverCategory.Storage;
        }

        if (upperPath.Contains("-SIO/", StringComparison.Ordinal))
        {
            return WortmannDriverCategory.SystemIo;
        }

        if (upperPath.Contains("-BT/", StringComparison.Ordinal))
        {
            return WortmannDriverCategory.Bluetooth;
        }

        if (upperPath.Contains("-WIFI/", StringComparison.Ordinal))
        {
            return WortmannDriverCategory.Wireless;
        }

        if (upperPath.Contains("-SOUND/", StringComparison.Ordinal))
        {
            return WortmannDriverCategory.Audio;
        }

        if (upperPath.Contains("/PC_VGA_", StringComparison.Ordinal))
        {
            return WortmannDriverCategory.Graphics;
        }

        return null;
    }

    private static Version ParseVersion(string fileName)
    {
        var match = VersionRegex().Match(fileName);

        return match.Success
            && Version.TryParse(match.Groups["version"].Value, out var version)
                ? version
                : new Version(0, 0);
    }

    private sealed record Candidate(
        WortmannDriverCategory Category,
        WortmannDownloadAsset Asset,
        int OsPreference,
        Version Version);

    [GeneratedRegex(
        @"(?<!\d)(?<version>\d{1,4}(?:\.\d+){1,3})(?!\d)",
        RegexOptions.CultureInvariant)]
    private static partial Regex VersionRegex();
}
