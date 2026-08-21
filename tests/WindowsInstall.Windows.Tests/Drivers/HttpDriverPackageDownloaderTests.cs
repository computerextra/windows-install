using System.Net;
using ComputerExtra.WindowsInstall.Windows.Drivers;

namespace ComputerExtra.WindowsInstall.Windows.Tests.Drivers;

[TestClass]
public sealed class HttpDriverPackageDownloaderTests
{
    [TestMethod]
    public async Task DownloadAsync_WritesResponseToDestination()
    {
        var root = CreateTempRoot();

        try
        {
            using var client = new HttpClient(
                new StubHandler(
                    HttpStatusCode.OK,
                    [1, 2, 3, 4]));

            var downloader =
                new HttpDriverPackageDownloader(client);

            var result = await downloader.DownloadAsync(
                new Uri("https://example.test/driver.zip"),
                root);

            Assert.AreEqual(4L, result.Length);
            Assert.AreEqual(
                "driver.zip",
                Path.GetFileName(result.FilePath));
            CollectionAssert.AreEqual(
                new byte[] { 1, 2, 3, 4 },
                await File.ReadAllBytesAsync(result.FilePath));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public async Task DownloadAsync_RejectsHttp()
    {
        using var client = new HttpClient(
            new StubHandler(
                HttpStatusCode.OK,
                [1]));

        var downloader =
            new HttpDriverPackageDownloader(client);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => downloader.DownloadAsync(
                new Uri("http://example.test/driver.zip"),
                Path.GetTempPath()).AsTask());
    }

    [TestMethod]
    public async Task DownloadAsync_PropagatesHttpFailure()
    {
        var root = CreateTempRoot();

        try
        {
            using var client = new HttpClient(
                new StubHandler(
                    HttpStatusCode.NotFound,
                    []));

            var downloader =
                new HttpDriverPackageDownloader(client);

            await Assert.ThrowsExactlyAsync<HttpRequestException>(
                () => downloader.DownloadAsync(
                    new Uri("https://example.test/missing.zip"),
                    root).AsTask());
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    private static string CreateTempRoot()
    {
        var root = Path.Combine(
            Path.GetTempPath(),
            $"WindowsInstall.DriverDownloadTests.{Guid.NewGuid():N}");

        Directory.CreateDirectory(root);
        return root;
    }

    private sealed class StubHandler(
        HttpStatusCode statusCode,
        byte[] content) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(
                new HttpResponseMessage(statusCode)
                {
                    Content = new ByteArrayContent(content)
                });
        }
    }
}
