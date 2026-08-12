using ComputerExtra.WindowsInstall.Core.Discovery;

namespace ComputerExtra.WindowsInstall.Core.Tests.Discovery;

[TestClass]
public sealed class SystemIdentityTests
{
    [TestMethod]
    public void Create_NormalizesValues()
    {
        var identity = SystemIdentity.Create(
            " TEST-PC ",
            " WORTMANN AG ",
            " TERRA MOBILE ",
            " ABC123 ");

        Assert.AreEqual("TEST-PC", identity.ComputerName);
        Assert.AreEqual("WORTMANN AG", identity.Manufacturer);
        Assert.AreEqual("TERRA MOBILE", identity.Model);
        Assert.AreEqual("ABC123", identity.DeviceSerialNumber);
    }

    [TestMethod]
    public void Create_ConvertsBlankOptionalValuesToNull()
    {
        var identity = SystemIdentity.Create(
            "TEST-PC",
            " ",
            null,
            string.Empty);

        Assert.IsNull(identity.Manufacturer);
        Assert.IsNull(identity.Model);
        Assert.IsNull(identity.DeviceSerialNumber);
    }

    [TestMethod]
    public void Create_RejectsMissingComputerName()
    {
        Assert.ThrowsExactly<ArgumentException>(
            () => SystemIdentity.Create(" ", null, null, null));
    }
}