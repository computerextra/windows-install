using ComputerExtra.WindowsInstall.Core.Discovery;

namespace ComputerExtra.WindowsInstall.Core.Tests.Discovery;

[TestClass]
public sealed class ManufacturerResolverTests
{
    [TestMethod]
    [DataRow("WORTMANN AG", ManufacturerId.Wortmann)]
    [DataRow("LENOVO", ManufacturerId.Lenovo)]
    [DataRow("ASUSTeK COMPUTER INC.", ManufacturerId.Asus)]
    [DataRow("ASUS", ManufacturerId.Asus)]
    [DataRow("Acer", ManufacturerId.Acer)]
    [DataRow("SCHENKER TECHNOLOGIES GMBH", ManufacturerId.Schenker)]
    [DataRow("Schenker Technologies GmbH", ManufacturerId.Schenker)]
    [DataRow("XMG", ManufacturerId.Xmg)]
    [DataRow("XMG GmbH", ManufacturerId.Xmg)]
    [DataRow("Other Vendor", ManufacturerId.Unsupported)]
    [DataRow(null, ManufacturerId.Unsupported)]
    public void Resolve_MapsKnownManufacturers(
        string? manufacturer,
        ManufacturerId expected)
    {
        Assert.AreEqual(expected, ManufacturerResolver.Resolve(manufacturer));
    }
}
