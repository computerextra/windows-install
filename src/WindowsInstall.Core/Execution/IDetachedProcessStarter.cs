namespace ComputerExtra.WindowsInstall.Core.Execution;

public interface IDetachedProcessStarter
{
    void Start(
        string fileName,
        IReadOnlyList<string> arguments);
}
