using ComputerExtra.WindowsInstall.Core.Execution;
using ComputerExtra.WindowsInstall.Core.Persistence;
using ComputerExtra.WindowsInstall.Core.Safety;

namespace ComputerExtra.WindowsInstall.Core.Tests.Persistence;

[TestClass]
public sealed class ResumeRuntimeTests
{
    [TestMethod]
    public void Create_BuildsSeparatedResumeRuntimePaths()
    {
        var layout = ResumeRuntimeLayout.Create(@"D:\ProgramData");

        Assert.AreEqual(
            @"D:\ProgramData\ComputerExtra\WindowsInstall\Resume",
            layout.RootDirectory);
        Assert.AreEqual(
            @"D:\ProgramData\ComputerExtra\WindowsInstall\Resume\WindowsInstall.exe",
            layout.ExecutablePath);
        Assert.AreEqual(
            @"D:\ProgramData\ComputerExtra\WindowsInstall\Resume\resume-state.json",
            layout.StatePath);
        Assert.AreEqual(
            @"D:\ProgramData\ComputerExtra\WindowsInstall\Resume\WindowsInstall.log",
            layout.LogPath);
    }

    [TestMethod]
    public void ResumeLaunchCommand_QuotesExecutableAndAddsResumeFlag()
    {
        var command = ResumeLaunchCommand.Create(
            @"C:\ProgramData\Computer Extra\WindowsInstall.exe");

        Assert.AreEqual(
            "\"C:\\ProgramData\\Computer Extra\\WindowsInstall.exe\" --resume",
            command);
    }

    [TestMethod]
    public async Task StageExecutableAsync_CopiesExecutableAtomically()
    {
        var testRoot = CreateTestRoot();

        try
        {
            var sourcePath = Path.Combine(testRoot, "source.exe");
            await File.WriteAllTextAsync(sourcePath, "resume-runtime");

            var layout = ResumeRuntimeLayout.Create(
                Path.Combine(testRoot, "ProgramData"));

            var stager = new ResumeRuntimeStager(
                layout,
                new AllowMutationGuard());

            await stager.StageExecutableAsync(sourcePath);

            Assert.IsTrue(File.Exists(layout.ExecutablePath));
            Assert.AreEqual(
                "resume-runtime",
                await File.ReadAllTextAsync(layout.ExecutablePath));
            Assert.IsFalse(File.Exists(layout.ExecutablePath + ".tmp"));
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [TestMethod]
    public async Task StageExecutableAsync_DevelopmentGuardBlocksMutation()
    {
        var testRoot = CreateTestRoot();

        try
        {
            var sourcePath = Path.Combine(testRoot, "source.exe");
            await File.WriteAllTextAsync(sourcePath, "resume-runtime");

            var layout = ResumeRuntimeLayout.Create(
                Path.Combine(testRoot, "ProgramData"));

            var stager = new ResumeRuntimeStager(
                layout,
                new DevelopmentSystemMutationGuard());

            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () => await stager.StageExecutableAsync(sourcePath));

            Assert.IsFalse(File.Exists(layout.ExecutablePath));
        }
        finally
        {
            Directory.Delete(testRoot, true);
        }
    }

    [TestMethod]
    public async Task ScheduledTaskRegistration_WithLayoutUsesStableResumeCommand()
    {
        var runner = new FakeProcessRunner();
        var layout = ResumeRuntimeLayout.Create(@"C:\ProgramData");

        var registration = new ScheduledTaskResumeRegistration(
            runner,
            new AllowMutationGuard(),
            layout);

        await registration.RegisterAsync();

        var arguments = runner.Arguments
            ?? throw new AssertFailedException("Task-Argumente wurden nicht erfasst.");
        var taskRunIndex = arguments.ToList().IndexOf("/tr");

        Assert.IsTrue(taskRunIndex >= 0);
        Assert.AreEqual(
            "\"C:\\ProgramData\\ComputerExtra\\WindowsInstall\\Resume\\WindowsInstall.exe\" --resume",
            arguments[taskRunIndex + 1]);
    }

    private static string CreateTestRoot()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            "ComputerExtra.WindowsInstall.Tests",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(path);
        return path;
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
        public IReadOnlyList<string>? Arguments { get; private set; }

        public ValueTask<ProcessResult> RunAsync(
            string fileName,
            IReadOnlyList<string> arguments,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Arguments = arguments.ToArray();

            return ValueTask.FromResult(
                new ProcessResult(0, "SUCCESS", ""));
        }
    }
}
