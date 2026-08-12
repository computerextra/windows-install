using ComputerExtra.WindowsInstall.Core.Safety;

namespace ComputerExtra.WindowsInstall.Core.Tests.Safety;

[TestClass]
public sealed class DevelopmentSystemMutationGuardTests
{
    [TestMethod]
    public void EnsureAllowed_BlocksPersistentSystemMutation()
    {
        var guard = new DevelopmentSystemMutationGuard();

        var exception = Assert.ThrowsExactly<InvalidOperationException>(
            () => guard.EnsureAllowed("Treiber installieren"));

        StringAssert.Contains(exception.Message, "blockiert");
        StringAssert.Contains(exception.Message, "Treiber installieren");
    }

    [TestMethod]
    public void EnsureAllowed_RejectsMissingOperationName()
    {
        var guard = new DevelopmentSystemMutationGuard();

        Assert.ThrowsExactly<ArgumentException>(
            () => guard.EnsureAllowed(" "));
    }
}