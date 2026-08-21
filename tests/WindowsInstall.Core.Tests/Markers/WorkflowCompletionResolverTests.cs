using ComputerExtra.WindowsInstall.Core.Markers;

namespace ComputerExtra.WindowsInstall.Core.Tests.Markers;

[TestClass]
public sealed class WorkflowCompletionResolverTests
{
    private static readonly MarkerDeviceContext CurrentDevice =
        MarkerDeviceContext.Create("WORTMANN AG", "ABC123");

    [TestMethod]
    public void RealSystemConfirmation_WinsWithoutMarker()
    {
        var markerStatus = WorkflowMarkerStatus.Create(
            [],
            CurrentDevice);

        Assert.IsTrue(
            WorkflowCompletionResolver.IsCompleted(
                markerStatus,
                "software.chrome",
                RealSystemStateEvidence.ConfirmsCompleted));
    }

    [TestMethod]
    public void RealSystemContradiction_OverridesValidMarker()
    {
        var markerStatus = CreateStatusWithValidMarker(
            "software.chrome");

        Assert.IsFalse(
            WorkflowCompletionResolver.IsCompleted(
                markerStatus,
                "software.chrome",
                RealSystemStateEvidence.ContradictsCompleted));
    }

    [TestMethod]
    public void ValidMarker_IsUsedWhenRealStateCannotBeReliablyVerified()
    {
        var markerStatus = CreateStatusWithValidMarker(
            "drivers.wortmann");

        Assert.IsTrue(
            WorkflowCompletionResolver.IsCompleted(
                markerStatus,
                "drivers.wortmann",
                RealSystemStateEvidence.NotReliablyVerifiable));
    }

    [TestMethod]
    public void MissingMarker_RemainsIncompleteWhenRealStateCannotBeReliablyVerified()
    {
        var markerStatus = WorkflowMarkerStatus.Create(
            [],
            CurrentDevice);

        Assert.IsFalse(
            WorkflowCompletionResolver.IsCompleted(
                markerStatus,
                "drivers.wortmann",
                RealSystemStateEvidence.NotReliablyVerifiable));
    }

    [TestMethod]
    public void DeviceMismatchedMarker_CannotProveCompletion()
    {
        var marker = WorkflowMarker.CreateCompleted(
            "drivers.wortmann",
            new DateTimeOffset(2026, 8, 21, 8, 0, 0, TimeSpan.Zero),
            "0.1.0",
            "WORTMANN AG",
            "OTHER-SERIAL");

        var markerStatus = WorkflowMarkerStatus.Create(
            [marker],
            CurrentDevice);

        Assert.IsFalse(
            WorkflowCompletionResolver.IsCompleted(
                markerStatus,
                "drivers.wortmann",
                RealSystemStateEvidence.NotReliablyVerifiable));
    }

    private static WorkflowMarkerStatus CreateStatusWithValidMarker(
        string workflowId)
    {
        var marker = WorkflowMarker.CreateCompleted(
            workflowId,
            new DateTimeOffset(2026, 8, 21, 8, 0, 0, TimeSpan.Zero),
            "0.1.0",
            "WORTMANN AG",
            "ABC123");

        return WorkflowMarkerStatus.Create(
            [marker],
            CurrentDevice);
    }
}
