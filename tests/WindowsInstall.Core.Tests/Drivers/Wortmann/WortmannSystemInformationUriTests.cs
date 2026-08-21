using ComputerExtra.WindowsInstall.Core.Drivers.Wortmann;

namespace ComputerExtra.WindowsInstall.Core.Tests.Drivers.Wortmann;

[TestClass]
public sealed class WortmannSystemInformationUriTests
{
    [TestMethod]
    public void Create_UsesSerialNumberAsPathKey()
    {
        var uri = WortmannSystemInformationUri.Create(" R7993106 ");

        Assert.AreEqual(
            "https://www.wortmann.de/de-de/systeminformation/R7993106.aspx",
            uri.AbsoluteUri);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("R7993106/other")]
    [DataRow("R7993106?x=1")]
    public void Create_RejectsInvalidSerialNumber(string serialNumber)
    {
        Assert.ThrowsExactly<ArgumentException>(
            () => WortmannSystemInformationUri.Create(serialNumber));
    }
}
