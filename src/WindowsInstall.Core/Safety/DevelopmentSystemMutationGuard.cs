namespace ComputerExtra.WindowsInstall.Core.Safety;

/// <summary>
/// Development guard used for local builds, tests and GUI work.
/// It deliberately rejects every persistent system mutation.
/// </summary>
public sealed class DevelopmentSystemMutationGuard : ISystemMutationGuard
{
    public void EnsureAllowed(string operation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);

        throw new InvalidOperationException(
            $"Systemänderung im lokalen Entwicklungsmodus blockiert: {operation}");
    }
}