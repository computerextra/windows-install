using System.Net;
using System.Text.RegularExpressions;

namespace ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

public static partial class WortmannDownloadAssetParser
{
    private const string TrustedHost = "webftp.wortmann.de";
    private const string TrustedPathPrefix = "/dokumentenmanagement_wag/";

    public static IReadOnlyList<WortmannDownloadAsset> Parse(string html)
    {
        ArgumentNullException.ThrowIfNull(html);

        var results = new Dictionary<string, WortmannDownloadAsset>(
            StringComparer.OrdinalIgnoreCase);

        foreach (Match match in HrefRegex().Matches(html))
        {
            var rawHref = WebUtility.HtmlDecode(
                match.Groups["href"].Value);

            if (!Uri.TryCreate(rawHref, UriKind.Absolute, out var uri))
            {
                continue;
            }

            if (!IsTrustedAssetUri(uri))
            {
                continue;
            }

            results.TryAdd(
                uri.AbsoluteUri,
                new WortmannDownloadAsset(uri));
        }

        return results.Values.ToArray();
    }

    public static bool IsTrustedAssetUri(Uri uri)
    {
        ArgumentNullException.ThrowIfNull(uri);

        return uri.Scheme == Uri.UriSchemeHttps
            && string.Equals(
                uri.Host,
                TrustedHost,
                StringComparison.OrdinalIgnoreCase)
            && uri.AbsolutePath.StartsWith(
                TrustedPathPrefix,
                StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrWhiteSpace(
                Path.GetFileName(uri.AbsolutePath));
    }

    [GeneratedRegex(
        """href\s*=\s*["'](?<href>[^"']+)["']""",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex HrefRegex();
}
