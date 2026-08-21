namespace ComputerExtra.WindowsInstall.Core.Execution;

public interface IProcessRunner
{
    ValueTask<ProcessResult> RunAsync(
        string fileName,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken = default);
}
