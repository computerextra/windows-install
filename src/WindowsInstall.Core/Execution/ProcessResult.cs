namespace ComputerExtra.WindowsInstall.Core.Execution;

public sealed record ProcessResult(
    int ExitCode,
    string StandardOutput,
    string StandardError);
