namespace ComputerExtra.WindowsInstall.Core.Discovery;

public sealed class ManufacturerIntegrationResolver(
    IEnumerable<IManufacturerIntegration> integrations)
{
    private readonly IReadOnlyList<IManufacturerIntegration> _integrations =
        integrations?.ToArray()
        ?? throw new ArgumentNullException(nameof(integrations));

    public IManufacturerIntegration? Resolve(SystemIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(identity);

        var manufacturer = ManufacturerResolver.Resolve(identity.Manufacturer);

        return _integrations.SingleOrDefault(
            integration =>
                integration.Manufacturer == manufacturer
                && integration.Supports(identity));
    }
}
