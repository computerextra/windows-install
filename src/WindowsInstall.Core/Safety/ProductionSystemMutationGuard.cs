namespace ComputerExtra.WindowsInstall.Core.Safety;

public sealed class ProductionSystemMutationGuard : ISystemMutationGuard
{
    public void EnsureAllowed(string operation)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(operation);
    }
}
