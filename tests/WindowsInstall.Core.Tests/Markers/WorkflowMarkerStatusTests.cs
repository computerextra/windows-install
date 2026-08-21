using ComputerExtra.WindowsInstall.Core.Markers;

namespace ComputerExtra.WindowsInstall.Core.Tests.Markers;

[TestClass]
public sealed class WorkflowMarkerStatusTests
{
    private static readonly MarkerDeviceContext CurrentDevice =
        MarkerDeviceContext.Create("WORTMANN AG", "ABC123");

    [TestMethod]
    public void IsCompleted_ReturnsTrueForValidMarker()
    {
        var status = WorkflowMarkerStatus.Create(
            [
                CreateMarker(
                    "wortmann-drivers",
                    new DateTimeOffset(2026, 8, 21, 8, 0, 0, TimeSpan.Zero))
            ],
            CurrentDevice);

        Assert.IsTrue(status.IsCompleted("wortmann-drivers"));
    }

    [TestMethod]
    public void IsCompleted_IgnoresMarkerForDifferentDevice()
    {
        var marker = WorkflowMarker.CreateCompleted(
            "wortmann-drivers",
            new DateTimeOffset(2026, 8, 21, 8, 0, 0, TimeSpan.Zero),
            "0.1.0",
            "WORTMANN AG",
            "OTHER");

        var status = WorkflowMarkerStatus.Create(
            [marker],
            CurrentDevice);

        Assert.IsFalse(status.IsCompleted("wortmann-drivers"));
    }

    [TestMethod]
    public void IsCompleted_IgnoresUnsupportedSchema()
    {
        var marker = new WorkflowMarker(
            WorkflowMarker.CurrentSchemaVersion + 1,
            "wortmann-drivers",
            new DateTimeOffset(2026, 8, 21, 8, 0, 0, TimeSpan.Zero),
            "0.1.0",
            "WORTMANN AG",
            "ABC123");

        var status = WorkflowMarkerStatus.Create(
            [marker],
            CurrentDevice);

        Assert.IsFalse(status.IsCompleted("wortmann-drivers"));
    }

    [TestMethod]
    public void GetCompletedMarker_UsesNewestValidMarkerForWorkflow()
    {
        var older = CreateMarker(
            "wortmann-drivers",
            new DateTimeOffset(2026, 8, 20, 8, 0, 0, TimeSpan.Zero));
        var newer = CreateMarker(
            "wortmann-drivers",
            new DateTimeOffset(2026, 8, 21, 8, 0, 0, TimeSpan.Zero));

        var status = WorkflowMarkerStatus.Create(
            [older, newer],
            CurrentDevice);

        Assert.AreSame(
            newer,
            status.GetCompletedMarker("wortmann-drivers"));
    }

    [TestMethod]
    public void WorkflowIds_AreMatchedCaseInsensitively()
    {
        var status = WorkflowMarkerStatus.Create(
            [
                CreateMarker(
                    "wortmann-drivers",
                    new DateTimeOffset(2026, 8, 21, 8, 0, 0, TimeSpan.Zero))
            ],
            CurrentDevice);

        Assert.IsTrue(status.IsCompleted("WORTMANN-DRIVERS"));
    }

    private static WorkflowMarker CreateMarker(
        string workflowId,
        DateTimeOffset completedAtUtc)
    {
        return WorkflowMarker.CreateCompleted(
            workflowId,
            completedAtUtc,
            "0.1.0",
            "WORTMANN AG",
            "ABC123");
    }
}
