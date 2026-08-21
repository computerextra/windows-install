using System.Windows;
using ComputerExtra.WindowsInstall.Core.Execution;
using ComputerExtra.WindowsInstall.Core.Persistence;
using ComputerExtra.WindowsInstall.Core.Safety;
using ComputerExtra.WindowsInstall.Core.State;

namespace ComputerExtra.WindowsInstall.App;

public partial class App : Application
{
    public const string SetupRunStateResourceKey = "WindowsInstall.SetupRunState";

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            if (e.Args.Contains("--resume", StringComparer.OrdinalIgnoreCase))
            {
                var runtimeLayout = ResumeRuntimeLayout.CreateDefault();
                var stateStore = new JsonFileSetupStateStore(runtimeLayout.StatePath);
                var resumeRegistration = new ScheduledTaskResumeRegistration(
                    new SystemProcessRunner(),
                    new ProductionSystemMutationGuard(),
                    runtimeLayout);
                var resumeCoordinator = new SetupResumeCoordinator(
                    stateStore,
                    resumeRegistration);
                var startup = new ResumeApplicationStartup(resumeCoordinator);

                var state = await startup.ResumeAsync();

                Resources[SetupRunStateResourceKey] = state;
            }

            MainWindow = new MainWindow();
            MainWindow.Show();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "WindowsInstall",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(1);
        }
    }
}
