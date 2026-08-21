using ComputerExtra.WindowsInstall.Core.Execution;
using ComputerExtra.WindowsInstall.Core.Safety;

namespace ComputerExtra.WindowsInstall.Core.Persistence;

public sealed class ScheduledTaskResumeRegistration(
    IProcessRunner processRunner,
    ISystemMutationGuard mutationGuard,
    string resumeCommand) : IResumeRegistration
{
    public const string TaskName = @"ComputerExtra\WindowsInstall.Resume";

    public ScheduledTaskResumeRegistration(
        IProcessRunner processRunner,
        ISystemMutationGuard mutationGuard,
        ResumeRuntimeLayout runtimeLayout)
        : this(
            processRunner,
            mutationGuard,
            ResumeLaunchCommand.Create(runtimeLayout.ExecutablePath))
    {
    }

    public async ValueTask RegisterAsync(
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resumeCommand);

        mutationGuard.EnsureAllowed("WindowsInstall-Resume-Task registrieren");

        var result = await processRunner.RunAsync(
            "schtasks.exe",
            [
                "/create",
                "/tn", TaskName,
                "/tr", resumeCommand,
                "/sc", "onlogon",
                "/rl", "highest",
                "/it",
                "/f"
            ],
            cancellationToken);

        EnsureSuccess(result, "registrieren");
    }

    public async ValueTask DeleteAsync(
        CancellationToken cancellationToken = default)
    {
        mutationGuard.EnsureAllowed("WindowsInstall-Resume-Task entfernen");

        var result = await processRunner.RunAsync(
            "schtasks.exe",
            [
                "/delete",
                "/tn", TaskName,
                "/f"
            ],
            cancellationToken);

        EnsureSuccess(result, "entfernen");
    }

    private static void EnsureSuccess(
        ProcessResult result,
        string operation)
    {
        if (result.ExitCode == 0)
        {
            return;
        }

        var detail = string.IsNullOrWhiteSpace(result.StandardError)
            ? result.StandardOutput
            : result.StandardError;

        throw new InvalidOperationException(
            $"WindowsInstall-Resume-Task konnte nicht {operation} werden. " +
            $"Exitcode: {result.ExitCode}. {detail}".Trim());
    }
}
