namespace ComputerExtra.WindowsInstall.Core.Discovery;

public interface ISystemIdentityReader
{
    ValueTask<SystemIdentity> ReadAsync(
        CancellationToken cancellationToken = default);
}
