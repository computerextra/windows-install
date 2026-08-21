using System.Text.Json;
using ComputerExtra.WindowsInstall.Core.State;

namespace ComputerExtra.WindowsInstall.Core.Persistence;

public sealed class JsonFileSetupStateStore(string statePath) : ISetupStateStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public async ValueTask<SetupRunState?> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(statePath))
        {
            return null;
        }

        await using var stream = new FileStream(
            statePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            FileOptions.Asynchronous);

        var document = await JsonSerializer.DeserializeAsync<SetupRunStateDocument>(
            stream,
            JsonOptions,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "Gespeicherter WindowsInstall-Resume-State ist leer oder ungültig.");

        return SetupRunState.Restore(
            document.SchemaVersion,
            document.CurrentStep,
            document.PendingReboot,
            document.IsCompleted,
            document.CompletedSteps ?? []);
    }

    public async ValueTask SaveAsync(
        SetupRunState state,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);

        var directory = Path.GetDirectoryName(statePath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException(
                "Resume-State-Pfad muss ein Verzeichnis enthalten.");
        }

        Directory.CreateDirectory(directory);

        var tempPath = statePath + ".tmp";

        var document = new SetupRunStateDocument
        {
            SchemaVersion = state.SchemaVersion,
            CurrentStep = state.CurrentStep,
            PendingReboot = state.PendingReboot,
            IsCompleted = state.IsCompleted,
            CompletedSteps = [.. state.CompletedSteps.Order()]
        };

        try
        {
            await using (var stream = new FileStream(
                tempPath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                4096,
                FileOptions.Asynchronous))
            {
                await JsonSerializer.SerializeAsync(
                    stream,
                    document,
                    JsonOptions,
                    cancellationToken);

                await stream.FlushAsync(cancellationToken);
            }

            File.Move(tempPath, statePath, true);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    public ValueTask DeleteAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (File.Exists(statePath))
        {
            File.Delete(statePath);
        }

        return ValueTask.CompletedTask;
    }

    private sealed class SetupRunStateDocument
    {
        public int SchemaVersion { get; init; }

        public WorkflowStepId? CurrentStep { get; init; }

        public bool PendingReboot { get; init; }

        public bool IsCompleted { get; init; }

        public WorkflowStepId[]? CompletedSteps { get; init; }
    }
}
