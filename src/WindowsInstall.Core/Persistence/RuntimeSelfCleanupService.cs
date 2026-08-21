using ComputerExtra.WindowsInstall.Core.Execution;
using ComputerExtra.WindowsInstall.Core.Safety;

namespace ComputerExtra.WindowsInstall.Core.Persistence;

public sealed class RuntimeSelfCleanupService(
    ISetupStateStore stateStore,
    ResumeRuntimeLayout runtimeLayout,
    IDetachedProcessStarter detachedProcessStarter,
    ISystemMutationGuard mutationGuard)
{
    public async ValueTask PrepareAfterSuccessfulRunAsync(
        int currentProcessId,
        CancellationToken cancellationToken = default)
    {
        if (currentProcessId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(currentProcessId));
        }

        mutationGuard.EnsureAllowed(
            "WindowsInstall-Laufzeitartefakte entfernen");

        await stateStore.DeleteAsync(cancellationToken);

        if (!Directory.Exists(runtimeLayout.RootDirectory))
        {
            return;
        }

        detachedProcessStarter.Start(
            "powershell.exe",
            RuntimeCleanupCommand.CreateArguments(
                runtimeLayout.RootDirectory,
                currentProcessId));
    }
}
