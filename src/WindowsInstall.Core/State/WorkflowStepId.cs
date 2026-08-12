namespace ComputerExtra.WindowsInstall.Core.State;

public enum WorkflowStepId
{
    InitialConfiguration = 0,
    SystemDetection = 10,
    DriverInstallation = 20,
    ComputerName = 30,
    OemInformation = 40,
    SoftwareInstallation = 50,
    DefaultApplications = 60,
    WindowsConfiguration = 70,
    StressTests = 80,
    Finalization = 90
}