using System.Net;
using System.Text.RegularExpressions;

namespace ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

public static partial class WortmannWindows11DriverAssetParser
{
    private const string DriverType = "Treiber";
    private const string Windows11X64 = "Windows 11 - 64 Bit";

    public static IReadOnlyList<WortmannDownloadAsset> Parse(string html)
    {
        ArgumentNullException.ThrowIfNull(html);

        var results = new Dictionary<string, WortmannDownloadAsset>(
            StringComparer.OrdinalIgnoreCase);

        foreach (Match rowMatch in TableRowRegex().Matches(html))
        {
            var row = rowMatch.Value;

            var typeMatch = TypeRegex().Match(row);
            if (!typeMatch.Success
                || !string.Equals(
                    DecodeText(typeMatch.Groups["value"].Value),
                    DriverType,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var operatingSystemsMatch =
                OperatingSystemsRegex().Match(row);

            if (!operatingSystemsMatch.Success
                || !ContainsWindows11X64(
                    operatingSystemsMatch.Groups["value"].Value))
            {
                continue;
            }

            foreach (var asset in WortmannDownloadAssetParser.Parse(row))
            {
                results.TryAdd(asset.DownloadUri.AbsoluteUri, asset);
            }
        }

        return results.Values.ToArray();
    }

    private static bool ContainsWindows11X64(string html)
    {
        foreach (Match itemMatch in ListItemRegex().Matches(html))
        {
            if (string.Equals(
                DecodeText(itemMatch.Groups["value"].Value),
                Windows11X64,
                StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string DecodeText(string value)
    {
        var withoutTags = HtmlTagRegex().Replace(value, string.Empty);

        return WebUtility.HtmlDecode(withoutTags).Trim();
    }

    [GeneratedRegex(
        """<tr\b[^>]*>(?<value>.*?)</tr>""",
        RegexOptions.IgnoreCase
        | RegexOptions.Singleline
        | RegexOptions.CultureInvariant)]
    private static partial Regex TableRowRegex();

    [GeneratedRegex(
        """<span\b[^>]*id=["'][^"']*_LabelType["'][^>]*>(?<value>.*?)</span>""",
        RegexOptions.IgnoreCase
        | RegexOptions.Singleline
        | RegexOptions.CultureInvariant)]
    private static partial Regex TypeRegex();

    [GeneratedRegex(
        """<ul\b[^>]*id=["'][^"']*_OperatingSystemsList["'][^>]*>(?<value>.*?)</ul>""",
        RegexOptions.IgnoreCase
        | RegexOptions.Singleline
        | RegexOptions.CultureInvariant)]
    private static partial Regex OperatingSystemsRegex();

    [GeneratedRegex(
        """<li\b[^>]*>(?<value>.*?)</li>""",
        RegexOptions.IgnoreCase
        | RegexOptions.Singleline
        | RegexOptions.CultureInvariant)]
    private static partial Regex ListItemRegex();

    [GeneratedRegex(
        """<[^>]+>""",
        RegexOptions.Singleline | RegexOptions.CultureInvariant)]
    private static partial Regex HtmlTagRegex();
}
