namespace ComputerExtra.WindowsInstall.Core.Discovery;

public interface ISystemDiscoveryService
{
    ValueTask<SetupSnapshot> DiscoverAsync(
        CancellationToken cancellationToken = default);
}