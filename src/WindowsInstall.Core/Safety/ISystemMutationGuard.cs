namespace ComputerExtra.WindowsInstall.Core.Safety;

/// <summary>
/// Guards operations that would persistently modify the local Windows system.
/// </summary>
public interface ISystemMutationGuard
{
    void EnsureAllowed(string operation);
}