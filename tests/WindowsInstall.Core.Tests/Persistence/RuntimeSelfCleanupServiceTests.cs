using ComputerExtra.WindowsInstall.Core.Execution;
using ComputerExtra.WindowsInstall.Core.Persistence;
using ComputerExtra.WindowsInstall.Core.Safety;
using ComputerExtra.WindowsInstall.Core.State;

namespace ComputerExtra.WindowsInstall.Core.Tests.Persistence;

[TestClass]
public sealed class RuntimeSelfCleanupServiceTests
{
    [TestMethod]
    public async Task PrepareAfterSuccessfulRunAsync_DeletesResumeState()
    {
        var layout = CreateLayout();
        var stateStore = new FakeStateStore();
        var service = new RuntimeSelfCleanupService(
            stateStore,
            layout,
            new FakeDetachedProcessStarter(),
            new AllowMutationGuard());

        await service.PrepareAfterSuccessfulRunAsync(1234);

        Assert.AreEqual(1, stateStore.DeleteCount);
    }

    [TestMethod]
    public async Task PrepareAfterSuccessfulRunAsync_SchedulesRuntimeDirectoryRemoval()
    {
        var layout = CreateLayout();
        Directory.CreateDirectory(layout.RootDirectory);

        try
        {
            var starter = new FakeDetachedProcessStarter();
            var service = new RuntimeSelfCleanupService(
                new FakeStateStore(),
                layout,
                starter,
                new AllowMutationGuard());

            await service.PrepareAfterSuccessfulRunAsync(1234);

            Assert.AreEqual("powershell.exe", starter.FileName);
            Assert.IsNotNull(starter.Arguments);
            Assert.IsTrue(
                starter.Arguments.Any(
                    argument => argument.Contains(
                        layout.RootDirectory,
                        StringComparison.Ordinal)));
        }
        finally
        {
            Directory.Delete(layout.RootDirectory, true);
        }
    }

    [TestMethod]
    public async Task PrepareAfterSuccessfulRunAsync_DoesNotStartCleanupWhenRuntimeIsAbsent()
    {
        var starter = new FakeDetachedProcessStarter();
        var service = new RuntimeSelfCleanupService(
            new FakeStateStore(),
            CreateLayout(),
            starter,
            new AllowMutationGuard());

        await service.PrepareAfterSuccessfulRunAsync(1234);

        Assert.IsNull(starter.FileName);
    }

    [TestMethod]
    public async Task PrepareAfterSuccessfulRunAsync_DevelopmentGuardBlocksMutation()
    {
        var stateStore = new FakeStateStore();
        var service = new RuntimeSelfCleanupService(
            stateStore,
            CreateLayout(),
            new FakeDetachedProcessStarter(),
            new DevelopmentSystemMutationGuard());

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await service.PrepareAfterSuccessfulRunAsync(1234));

        Assert.AreEqual(0, stateStore.DeleteCount);
    }

    [TestMethod]
    public void RuntimeCleanupCommand_WaitsForProcessAndDeletesExactDirectory()
    {
        var arguments = RuntimeCleanupCommand.CreateArguments(
            @"C:\ProgramData\ComputerExtra\WindowsInstall\Resume",
            4321);

        Assert.AreEqual("-Command", arguments[^2]);
        StringAssert.Contains(arguments[^1], "Get-Process -Id 4321");
        StringAssert.Contains(
            arguments[^1],
            "C:\\ProgramData\\ComputerExtra\\WindowsInstall\\Resume");
    }

    private static ResumeRuntimeLayout CreateLayout()
    {
        return ResumeRuntimeLayout.Create(
            Path.Combine(
                Path.GetTempPath(),
                "ComputerExtra.WindowsInstall.Tests",
                Guid.NewGuid().ToString("N")));
    }

    private sealed class FakeStateStore : ISetupStateStore
    {
        public int DeleteCount { get; private set; }

        public ValueTask<SetupRunState?> LoadAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult<SetupRunState?>(null);
        }

        public ValueTask SaveAsync(
            SetupRunState state,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.CompletedTask;
        }

        public ValueTask DeleteAsync(
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            DeleteCount++;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class FakeDetachedProcessStarter : IDetachedProcessStarter
    {
        public string? FileName { get; private set; }

        public IReadOnlyList<string>? Arguments { get; private set; }

        public void Start(
            string fileName,
            IReadOnlyList<string> arguments)
        {
            FileName = fileName;
            Arguments = arguments.ToArray();
        }
    }

    private sealed class AllowMutationGuard : ISystemMutationGuard
    {
        public void EnsureAllowed(string operation)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(operation);
        }
    }
}
