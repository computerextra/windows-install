using ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

namespace ComputerExtra.WindowsInstall.Core.Tests.Drivers.Wortmann;

[TestClass]
public sealed class WortmannDownloadAssetParserTests
{
    [TestMethod]
    public void Parse_ReturnsOnlyTrustedWortmannDownloadAssets()
    {
        const string html =
            """
            <a href="https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-CHIP/Intel_Chipset.exe">Chipset</a>
            <a href="https://webftp.wortmann.de/dokumentenmanagement_wag/PC_MB_ASUS_TGZ790PW-LAN/Intel_LAN.zip">LAN</a>
            <a href="https://example.com/dokumentenmanagement_wag/fake.exe">Fake</a>
            <a href="http://webftp.wortmann.de/dokumentenmanagement_wag/insecure.exe">Insecure</a>
            <a href="https://webftp.wortmann.de/other/not-trusted.exe">Other</a>
            """;

        var assets = WortmannDownloadAssetParser.Parse(html);

        Assert.HasCount(2, assets);
        CollectionAssert.AreEquivalent(
            new[] { "Intel_Chipset.exe", "Intel_LAN.zip" },
            assets.Select(asset => asset.FileName).ToArray());
    }

    [TestMethod]
    public void Parse_DecodesHtmlEntitiesAndRemovesDuplicates()
    {
        const string html =
            """
            <a href="https://webftp.wortmann.de/dokumentenmanagement_wag/driver/file.exe?x=1&amp;y=2">One</a>
            <a href="https://webftp.wortmann.de/dokumentenmanagement_wag/driver/file.exe?x=1&amp;y=2">Two</a>
            """;

        var assets = WortmannDownloadAssetParser.Parse(html);

        Assert.HasCount(1, assets);
        Assert.AreEqual(
            "https://webftp.wortmann.de/dokumentenmanagement_wag/driver/file.exe?x=1&y=2",
            assets.Single().DownloadUri.AbsoluteUri);
    }

    [TestMethod]
    public void Parse_ReturnsEmptyCollectionWhenNoDownloadAssetsExist()
    {
        var assets = WortmannDownloadAssetParser.Parse(
            "<html><body>Keine Downloads</body></html>");

        Assert.HasCount(0, assets);
    }

    [TestMethod]
    public void IsTrustedAssetUri_RejectsHostSuffixAttack()
    {
        var uri = new Uri(
            "https://webftp.wortmann.de.attacker.example/dokumentenmanagement_wag/file.exe");

        Assert.IsFalse(
            WortmannDownloadAssetParser.IsTrustedAssetUri(uri));
    }
}
