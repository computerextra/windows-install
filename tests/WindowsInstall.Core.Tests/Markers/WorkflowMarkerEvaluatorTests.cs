using ComputerExtra.WindowsInstall.Core.Markers;

namespace ComputerExtra.WindowsInstall.Core.Tests.Markers;

[TestClass]
public sealed class WorkflowMarkerEvaluatorTests
{
    private static readonly DateTimeOffset CompletedAt =
        new(2026, 8, 12, 12, 0, 0, TimeSpan.Zero);

    [TestMethod]
    public void Evaluate_AcceptsMarkerForSameDevice()
    {
        var marker = WorkflowMarker.CreateCompleted(
            "wortmann-drivers",
            CompletedAt,
            "0.1.0",
            "WORTMANN AG",
            "ABC123");

        var device = MarkerDeviceContext.Create(
            "wortmann ag",
            "abc123");

        Assert.AreEqual(
            MarkerMatchResult.Valid,
            WorkflowMarkerEvaluator.Evaluate(marker, device));
    }

    [TestMethod]
    public void Evaluate_RejectsDifferentManufacturer()
    {
        var marker = WorkflowMarker.CreateCompleted(
            "wortmann-drivers",
            CompletedAt,
            "0.1.0",
            "WORTMANN AG",
            "ABC123");

        var device = MarkerDeviceContext.Create(
            "Lenovo",
            "ABC123");

        Assert.AreEqual(
            MarkerMatchResult.ManufacturerMismatch,
            WorkflowMarkerEvaluator.Evaluate(marker, device));
    }

    [TestMethod]
    public void Evaluate_RejectsDifferentSerialNumber()
    {
        var marker = WorkflowMarker.CreateCompleted(
            "wortmann-drivers",
            CompletedAt,
            "0.1.0",
            "WORTMANN AG",
            "ABC123");

        var device = MarkerDeviceContext.Create(
            "WORTMANN AG",
            "XYZ999");

        Assert.AreEqual(
            MarkerMatchResult.DeviceSerialNumberMismatch,
            WorkflowMarkerEvaluator.Evaluate(marker, device));
    }

    [TestMethod]
    public void Evaluate_RejectsUnsupportedSchema()
    {
        var marker = new WorkflowMarker(
            WorkflowMarker.CurrentSchemaVersion + 1,
            "wortmann-drivers",
            CompletedAt,
            "0.1.0",
            "WORTMANN AG",
            "ABC123");

        var device = MarkerDeviceContext.Create(
            "WORTMANN AG",
            "ABC123");

        Assert.AreEqual(
            MarkerMatchResult.UnsupportedSchema,
            WorkflowMarkerEvaluator.Evaluate(marker, device));
    }

    [TestMethod]
    public void CreateCompleted_NormalizesUtcTimestampAndText()
    {
        var marker = WorkflowMarker.CreateCompleted(
            " wortmann-drivers ",
            new DateTimeOffset(2026, 8, 12, 14, 0, 0, TimeSpan.FromHours(2)),
            " 0.1.0 ",
            " WORTMANN AG ",
            " ABC123 ");

        Assert.AreEqual("wortmann-drivers", marker.WorkflowId);
        Assert.AreEqual("0.1.0", marker.InstallerVersion);
        Assert.AreEqual("WORTMANN AG", marker.Manufacturer);
        Assert.AreEqual("ABC123", marker.DeviceSerialNumber);
        Assert.AreEqual(TimeSpan.Zero, marker.CompletedAtUtc.Offset);
        Assert.AreEqual(12, marker.CompletedAtUtc.Hour);
    }
}