namespace ComputerExtra.WindowsInstall.Core.Persistence;

public interface IResumeRegistration
{
    ValueTask RegisterAsync(CancellationToken cancellationToken = default);

    ValueTask DeleteAsync(CancellationToken cancellationToken = default);
}
