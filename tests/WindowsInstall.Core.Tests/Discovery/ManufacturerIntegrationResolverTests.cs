using ComputerExtra.WindowsInstall.Core.Discovery;

namespace ComputerExtra.WindowsInstall.Core.Tests.Discovery;

[TestClass]
public sealed class ManufacturerIntegrationResolverTests
{
    [TestMethod]
    [DataRow("WORTMANN AG", ManufacturerId.Wortmann)]
    [DataRow("LENOVO", ManufacturerId.Lenovo)]
    [DataRow("ASUSTeK COMPUTER INC.", ManufacturerId.Asus)]
    [DataRow("Acer", ManufacturerId.Acer)]
    public void Resolve_SelectsMatchingIntegration(
        string manufacturer,
        ManufacturerId expectedManufacturer)
    {
        var integrations = Enum.GetValues<ManufacturerId>()
            .Where(value => value != ManufacturerId.Unsupported)
            .Select(value => new FakeIntegration(value))
            .ToArray();
        var resolver = new ManufacturerIntegrationResolver(integrations);
        var identity = SystemIdentity.Create(
            "PC",
            manufacturer,
            "Model",
            "123",
            DeviceClass.Desktop);

        var integration = resolver.Resolve(identity);

        Assert.IsNotNull(integration);
        Assert.AreEqual(expectedManufacturer, integration.Manufacturer);
    }

    [TestMethod]
    public void Resolve_ReturnsNullForUnsupportedManufacturer()
    {
        var resolver = new ManufacturerIntegrationResolver(
            [
                new FakeIntegration(ManufacturerId.Wortmann),
                new FakeIntegration(ManufacturerId.Lenovo),
                new FakeIntegration(ManufacturerId.Asus),
                new FakeIntegration(ManufacturerId.Acer)
            ]);
        var identity = SystemIdentity.Create(
            "PC",
            "Unknown",
            "Model",
            "123",
            DeviceClass.Desktop);

        Assert.IsNull(resolver.Resolve(identity));
    }

    private sealed class FakeIntegration(ManufacturerId manufacturer)
        : IManufacturerIntegration
    {
        public ManufacturerId Manufacturer { get; } = manufacturer;

        public bool Supports(SystemIdentity identity)
        {
            ArgumentNullException.ThrowIfNull(identity);
            return true;
        }
    }
}
