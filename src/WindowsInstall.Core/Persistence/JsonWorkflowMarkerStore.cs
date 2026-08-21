using System.Text.Json;
using ComputerExtra.WindowsInstall.Core.Markers;
using ComputerExtra.WindowsInstall.Core.Safety;

namespace ComputerExtra.WindowsInstall.Core.Persistence;

public sealed class JsonWorkflowMarkerStore(
    string markerPath,
    ISystemMutationGuard mutationGuard) : IWorkflowMarkerStore
{
    public const int CurrentDocumentSchemaVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public JsonWorkflowMarkerStore(ISystemMutationGuard mutationGuard)
        : this(MarkerFileDefinition.FullPath, mutationGuard)
    {
    }

    public async ValueTask<IReadOnlyCollection<WorkflowMarker>> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(markerPath))
        {
            return Array.Empty<WorkflowMarker>();
        }

        await using var stream = new FileStream(
            markerPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            4096,
            FileOptions.Asynchronous);

        var document = await JsonSerializer.DeserializeAsync<MarkerDocument>(
            stream,
            JsonOptions,
            cancellationToken)
            ?? throw new InvalidOperationException(
                "WindowsInstall-Markerdatei ist leer oder ungültig.");

        if (document.SchemaVersion != CurrentDocumentSchemaVersion)
        {
            throw new InvalidOperationException(
                $"Nicht unterstützte Markerdatei-Schema-Version: {document.SchemaVersion}.");
        }

        return document.Markers ?? Array.Empty<WorkflowMarker>();
    }

    public async ValueTask SaveAsync(
        IReadOnlyCollection<WorkflowMarker> markers,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(markers);

        mutationGuard.EnsureAllowed(
            "WindowsInstall-Workflow-Marker speichern");

        var directory = Path.GetDirectoryName(markerPath);

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException(
                "Markerdatei-Pfad muss ein Verzeichnis enthalten.");
        }

        Directory.CreateDirectory(directory);

        var tempPath = markerPath + ".tmp";
        var document = new MarkerDocument
        {
            SchemaVersion = CurrentDocumentSchemaVersion,
            Markers = [.. markers]
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

            File.Move(tempPath, markerPath, true);

            if (MarkerFileDefinition.MustBeHidden)
            {
                File.SetAttributes(
                    markerPath,
                    File.GetAttributes(markerPath) | FileAttributes.Hidden);
            }
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    private sealed class MarkerDocument
    {
        public int SchemaVersion { get; init; }

        public WorkflowMarker[]? Markers { get; init; }
    }
}
