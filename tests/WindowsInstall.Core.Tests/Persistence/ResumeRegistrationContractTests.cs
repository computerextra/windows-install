using ComputerExtra.WindowsInstall.Core.Persistence;

namespace ComputerExtra.WindowsInstall.Core.Tests.Persistence;

[TestClass]
public sealed class ResumeRegistrationContractTests
{
    [TestMethod]
    public void Contract_ExposesRegisterAndDeleteOperations()
    {
        var methods = typeof(IResumeRegistration)
            .GetMethods()
            .Select(method => method.Name)
            .Order()
            .ToArray();

        CollectionAssert.AreEqual(
            new[] { "DeleteAsync", "RegisterAsync" },
            methods);
    }
}
