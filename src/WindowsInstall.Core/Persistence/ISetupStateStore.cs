using ComputerExtra.WindowsInstall.Core.State;

namespace ComputerExtra.WindowsInstall.Core.Persistence;

public interface ISetupStateStore
{
    ValueTask<SetupRunState?> LoadAsync(CancellationToken cancellationToken = default);

    ValueTask SaveAsync(
        SetupRunState state,
        CancellationToken cancellationToken = default);

    ValueTask DeleteAsync(CancellationToken cancellationToken = default);
}