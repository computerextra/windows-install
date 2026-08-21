using System.Diagnostics;
using ComputerExtra.WindowsInstall.Core.Execution;
using ComputerExtra.WindowsInstall.Core.Persistence;
using ComputerExtra.WindowsInstall.Core.Safety;
using ComputerExtra.WindowsInstall.Core.State;

if (args.Length != 1)
{
    Console.Error.WriteLine("Genau ein Runtime-Verzeichnis muss angegeben werden.");
    return 2;
}

var runtimeRoot = Path.GetFullPath(args[0]);

if (Directory.Exists(runtimeRoot))
{
    Directory.Delete(runtimeRoot, true);
}

Directory.CreateDirectory(runtimeRoot);

var layout = new ResumeRuntimeLayout(
    runtimeRoot,
    Path.Combine(runtimeRoot, "WindowsInstall.exe"),
    Path.Combine(runtimeRoot, "resume-state.json"),
    Path.Combine(runtimeRoot, "WindowsInstall.log"));

await File.WriteAllTextAsync(layout.ExecutablePath, "integration-runtime");
await File.WriteAllTextAsync(layout.LogPath, "integration-log");

var stateStore = new JsonFileSetupStateStore(layout.StatePath);
var state = new SetupRunState();

await stateStore.SaveAsync(state);

var cleanupService = new RuntimeSelfCleanupService(
    stateStore,
    layout,
    new SystemDetachedProcessStarter(),
    new ProductionSystemMutationGuard());

var completionCoordinator = new SetupRunCompletionCoordinator(
    stateStore,
    new NoOpResumeRegistration(),
    cleanupService);

await completionCoordinator.CompleteAsync(
    state,
    Environment.ProcessId);

if (!state.IsCompleted)
{
    Console.Error.WriteLine("Workflow wurde nicht als abgeschlossen markiert.");
    return 3;
}

if (File.Exists(layout.StatePath))
{
    Console.Error.WriteLine("Resume-State wurde nicht gelöscht.");
    return 4;
}

Console.WriteLine(runtimeRoot);
return 0;

sealed class NoOpResumeRegistration : IResumeRegistration
{
    public ValueTask RegisterAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.CompletedTask;
    }
}
