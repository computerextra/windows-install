using ComputerExtra.WindowsInstall.Core.Discovery;

namespace ComputerExtra.WindowsInstall.Core.Tests.Discovery;

[TestClass]
public sealed class SystemIdentityResolverTests
{
    [TestMethod]
    [DataRow((ushort)1, (ushort)1, DeviceClass.Desktop)]
    [DataRow((ushort)2, (ushort)2, DeviceClass.Mobile)]
    [DataRow((ushort)3, (ushort)3, DeviceClass.Workstation)]
    [DataRow((ushort)6, (ushort)6, DeviceClass.Appliance)]
    [DataRow((ushort)2, (ushort)8, DeviceClass.Tablet)]
    [DataRow((ushort)0, (ushort)0, DeviceClass.Unknown)]
    [DataRow((ushort)7, (ushort)7, DeviceClass.Unknown)]
    public void ResolveDeviceClass_MapsWindowsPcSystemTypes(
        ushort pcSystemType,
        ushort pcSystemTypeEx,
        DeviceClass expected)
    {
        Assert.AreEqual(
            expected,
            SystemIdentityResolver.ResolveDeviceClass(
                pcSystemType,
                pcSystemTypeEx));
    }

    [TestMethod]
    public void Resolve_NormalizesWindowsFacts()
    {
        var identity = SystemIdentityResolver.Resolve(
            new WindowsComputerSystemFacts(
                " PC-01 ",
                " WORTMANN AG ",
                " TERRA ",
                " 123456 ",
                2,
                2));

        Assert.AreEqual("PC-01", identity.ComputerName);
        Assert.AreEqual("WORTMANN AG", identity.Manufacturer);
        Assert.AreEqual("TERRA", identity.Model);
        Assert.AreEqual("123456", identity.DeviceSerialNumber);
        Assert.AreEqual(DeviceClass.Mobile, identity.DeviceClass);
    }
}
