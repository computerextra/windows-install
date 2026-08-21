using ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

namespace ComputerExtra.WindowsInstall.Core.Tests.Drivers.Wortmann;

[TestClass]
public sealed class WortmannWindows11DriverAssetParserTests
{
    [TestMethod]
    public void Parse_ReturnsOnlyWindows11DriverRows()
    {
        const string html =
            """
            <table>
              <tr>
                <td><span id="x_LabelType">Treiber</span></td>
                <td><ul id="x_OperatingSystemsList"><li>Windows 10 - 64 Bit</li><li>Windows 11 - 64 Bit</li></ul></td>
                <td><a href="https://webftp.wortmann.de/dokumentenmanagement_wag/chip/chipset.zip">Download</a></td>
              </tr>
              <tr>
                <td><span id="y_LabelType">Treiber</span></td>
                <td><ul id="y_OperatingSystemsList"><li>Windows 10 - 64 Bit</li></ul></td>
                <td><a href="https://webftp.wortmann.de/dokumentenmanagement_wag/lan/win10.zip">Download</a></td>
              </tr>
              <tr>
                <td><span id="z_LabelType">Utility</span></td>
                <td><ul id="z_OperatingSystemsList"><li>Windows 11 - 64 Bit</li></ul></td>
                <td><a href="https://webftp.wortmann.de/dokumentenmanagement_wag/tool/tool.zip">Download</a></td>
              </tr>
              <tr>
                <td><span id="m_LabelType">Handbuch</span></td>
                <td><ul id="m_OperatingSystemsList"><li>unabhängig</li></ul></td>
                <td><a href="https://webftp.wortmann.de/dokumentenmanagement_wag/manual/manual.pdf">Download</a></td>
              </tr>
            </table>
            """;

        var assets =
            WortmannWindows11DriverAssetParser.Parse(html);

        Assert.HasCount(1, assets);
        Assert.AreEqual("chipset.zip", assets.Single().FileName);
    }

    [TestMethod]
    public void Parse_HandlesEncodedText()
    {
        const string html =
            """
            <tr>
              <td><span id="x_LabelType">Treiber</span></td>
              <td><ul id="x_OperatingSystemsList"><li>Windows 11 - 64 Bit</li></ul></td>
              <td><a href="https://webftp.wortmann.de/dokumentenmanagement_wag/driver/file.zip?x=1&amp;y=2">Download</a></td>
            </tr>
            """;

        var assets =
            WortmannWindows11DriverAssetParser.Parse(html);

        Assert.HasCount(1, assets);
        Assert.AreEqual(
            "https://webftp.wortmann.de/dokumentenmanagement_wag/driver/file.zip?x=1&y=2",
            assets.Single().DownloadUri.AbsoluteUri);
    }

    [TestMethod]
    public void Parse_ReturnsEmptyForPageWithoutMatchingRows()
    {
        Assert.HasCount(
            0,
            WortmannWindows11DriverAssetParser.Parse(
                "<html><body>Keine Treiber</body></html>"));
    }
}
