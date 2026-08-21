using ComputerExtra.WindowsInstall.Core.Execution;
using ComputerExtra.WindowsInstall.Core.Persistence;
using ComputerExtra.WindowsInstall.Core.Safety;

namespace ComputerExtra.WindowsInstall.Core.Tests.Persistence;

[TestClass]
public sealed class ScheduledTaskResumeRegistrationTests
{
    [TestMethod]
    public async Task RegisterAsync_UsesInteractiveHighestOnLogonTask()
    {
        var runner = new FakeProcessRunner();
        var guard = new AllowMutationGuard();
        var registration = new ScheduledTaskResumeRegistration(
            runner,
            guard,
            "powershell.exe -NoProfile -File C:\\Resume.ps1");

        await registration.RegisterAsync();

        Assert.AreEqual("schtasks.exe", runner.FileName);
        CollectionAssert.AreEqual(
            new[]
            {
                "/create",
                "/tn", ScheduledTaskResumeRegistration.TaskName,
                "/tr", "powershell.exe -NoProfile -File C:\\Resume.ps1",
                "/sc", "onlogon",
                "/rl", "highest",
                "/it",
                "/f"
            },
            runner.Arguments!.ToArray());
    }

    [TestMethod]
    public async Task DeleteAsync_RemovesExactProjectTask()
    {
        var runner = new FakeProcessRunner();
        var registration = new ScheduledTaskResumeRegistration(
            runner,
            new AllowMutationGuard(),
            "resume");

        await registration.DeleteAsync();

        CollectionAssert.AreEqual(
            new[]
            {
                "/delete",
                "/tn", ScheduledTaskResumeRegistration.TaskName,
                "/f"
            },
            runner.Arguments!.ToArray());
    }

    [TestMethod]
    public async Task RegisterAsync_DevelopmentGuardBlocksPersistentMutation()
    {
        var runner = new FakeProcessRunner();
        var registration = new ScheduledTaskResumeRegistration(
            runner,
            new DevelopmentSystemMutationGuard(),
            "resume");

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await registration.RegisterAsync());

        Assert.IsNull(runner.FileName);
    }

    [TestMethod]
    public async Task RegisterAsync_RejectsFailedSchtasksCommand()
    {
        var runner = new FakeProcessRunner
        {
            Result = new ProcessResult(1, "", "ACCESS DENIED")
        };
        var registration = new ScheduledTaskResumeRegistration(
            runner,
            new AllowMutationGuard(),
            "resume");

        var exception = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await registration.RegisterAsync());

        StringAssert.Contains(exception.Message, "Exitcode: 1");
        StringAssert.Contains(exception.Message, "ACCESS DENIED");
    }

    private sealed class AllowMutationGuard : ISystemMutationGuard
    {
        public void EnsureAllowed(string operation)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        }
    }

    private sealed class FakeProcessRunner : IProcessRunner
    {
        public ProcessResult Result { get; init; } =
            new(0, "SUCCESS", "");

        public string? FileName { get; private set; }

        public IReadOnlyList<string>? Arguments { get; private set; }

        public ValueTask<ProcessResult> RunAsync(
            string fileName,
            IReadOnlyList<string> arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            FileName = fileName;
            Arguments = arguments.ToArray();
            return ValueTask.FromResult(Result);
        }
    }
}
