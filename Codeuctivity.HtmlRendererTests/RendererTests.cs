using Codeuctivity.HtmlRenderer;
using Codeuctivity.HtmlRendererTests.Infrastructure;
using Codeuctivity.PdfjsSharp;
using Jering.Javascript.NodeJS;
using PuppeteerSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Codeuctivity.HtmlRendererTests
{
    public class RendererTests : IDisposable
    {
        private bool disposedValue;

        public RendererTests()
        {
            Rasterize = new Rasterizer();
        }

        public Rasterizer Rasterize { get; private set; }

        [Theory]
        [InlineData("BasicTextFormatted.html")]
        public async Task ShouldConvertHtmlToPdf(string testFileName)
        {
            var sourceHtmlFilePath = $"../../../TestInput/{testFileName}";
            var actualFilePath = Path.Combine(Path.GetTempPath(), $"ActualConvertHtmlToPdf{testFileName}.pdf");
            var expectReferenceFilePath = $"../../../ExpectedTestOutcome/ExpectedFromHtmlConvertHtmlToPdf{testFileName}.png";

            if (File.Exists(actualFilePath))
            {
                File.Delete(actualFilePath);
            }

            await using (var chromiumRenderer = await Renderer.CreateAsync())
            {
                await chromiumRenderer.ConvertHtmlToPdf(sourceHtmlFilePath, actualFilePath);

                var actualImagePathDirectory = Path.Combine(Path.GetTempPath(), testFileName);

                if (!IsRunningOnAzureOrMacos())
                {
                    var actualImages = await Rasterize.ConvertToPngAsync(actualFilePath, actualImagePathDirectory);
                    Assert.Single(actualImages);
                    // File.Copy(actualImages.Single(), expectReferenceFilePath, true);
                    DocumentAsserter.AssertImageIsEqual(actualImages.Single(), expectReferenceFilePath, 8000);
                }
                File.Delete(actualFilePath);
            }
            await ChromiumProcessDisposedAsserter.AssertNoChromiumProcessIsRunning();
        }

        [Theory]
        [InlineData("BasicTextFormattedInlineBackground.html", false, 9000)]
        [InlineData("BasicTextFormattedInlineBackground.html", true, 9000)]
        public async Task ShouldConvertHtmlToPdfWithOptions(string testFileName, bool printBackground, int allowedPixelDiff)
        {
            var sourceHtmlFilePath = $"../../../TestInput/{testFileName}";
            var actualFilePath = Path.Combine(Path.GetTempPath(), $"ActualConvertHtmlToPdf{testFileName}.{printBackground}.pdf");
            var expectReferenceFilePath = $"../../../ExpectedTestOutcome/ExpectedFromHtmlConvertHtmlToPdf{testFileName}.{printBackground}.png";

            if (File.Exists(actualFilePath))
            {
                File.Delete(actualFilePath);
            }

            await using (var chromiumRenderer = await Renderer.CreateAsync())
            {
                await chromiumRenderer.ConvertHtmlToPdf(sourceHtmlFilePath, actualFilePath, new PdfOptions() { PrintBackground = printBackground });

                var actualImagePathDirectory = Path.Combine(Path.GetTempPath(), testFileName);

                if (!IsRunningOnAzureOrMacos())
                {
                    try
                    {
                        var actualImages = await Rasterize.ConvertToPngAsync(actualFilePath, actualImagePathDirectory);
                        Assert.Single(actualImages);
                        // File.Copy(actualImages.Single(), expectReferenceFilePath, true);
                        DocumentAsserter.AssertImageIsEqual(actualImages.Single(), expectReferenceFilePath, allowedPixelDiff);
                    }
                    catch (InvocationException ex)
                    {
                        // Working around issue in Jering.Javascript.NodeJS, silencing false positive failing
                        Assert.True(RuntimeInformation.IsOSPlatform(OSPlatform.Windows), ex.Message);
                    }
                }
                File.Delete(actualFilePath);
            }
            await ChromiumProcessDisposedAsserter.AssertNoChromiumProcessIsRunning();
        }

        private static bool IsRunningOnAzureOrMacos()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                return true;
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return false;
            }

            var version = File.ReadAllText("/proc/version");
            var IsAzure = version.IndexOf("azure", StringComparison.OrdinalIgnoreCase) >= 0;

            return IsAzure;
        }

        [Theory]
        [InlineData("BasicTextFormatted.html")]
        public async Task ShouldConvertHtmlToPng(string testFileName)
        {
            var sourceHtmlFilePath = $"../../../TestInput/{testFileName}";
            var actualFilePath = Path.Combine(Path.GetTempPath(), $"ActualConvertHtmlToPng{testFileName}.png");
            var expectReferenceFilePath = $"../../../ExpectedTestOutcome/ExpectedConvertHtmlToPng{testFileName}.png";

            if (File.Exists(actualFilePath))
            {
                File.Delete(actualFilePath);
            }

            await using (var chromiumRenderer = await Renderer.CreateAsync())
            {
                await chromiumRenderer.ConvertHtmlToPng(sourceHtmlFilePath, actualFilePath);

                DocumentAsserter.AssertImageIsEqual(actualFilePath, expectReferenceFilePath, 9000);
            }

            File.Delete(actualFilePath);
            await ChromiumProcessDisposedAsserter.AssertNoChromiumProcessIsRunning();
        }

        [Theory]
        [InlineData("BasicTextFormattedInlineBackground.html", false, 15000)]
        [InlineData("BasicTextFormattedInlineBackground.html", true, 9500)]
        public async Task ShouldConvertHtmlToPngScreenshotOptions(string testFileName, bool omitBackground, int allowedPixelDiff)
        {
            var sourceHtmlFilePath = $"../../../TestInput/{testFileName}";
            var actualFilePath = Path.Combine(Path.GetTempPath(), $"ActualConvertHtmlToPng{testFileName}.{omitBackground}.png");
            var expectReferenceFilePath = $"../../../ExpectedTestOutcome/ExpectedConvertHtmlToPng{testFileName}.{omitBackground}.png";

            if (File.Exists(actualFilePath))
            {
                File.Delete(actualFilePath);
            }

            await using (var chromiumRenderer = await Renderer.CreateAsync())
            {
                ScreenshotOptions screenshotOptions = new ScreenshotOptions
                {
                    OmitBackground = omitBackground
                };

                await chromiumRenderer.ConvertHtmlToPng(sourceHtmlFilePath, actualFilePath, screenshotOptions);
                // File.Copy(actualFilePath, expectReferenceFilePath, true);
                DocumentAsserter.AssertImageIsEqual(actualFilePath, expectReferenceFilePath, allowedPixelDiff);
            }

            File.Delete(actualFilePath);
            await ChromiumProcessDisposedAsserter.AssertNoChromiumProcessIsRunning();
        }

        [Theory]
        [InlineData("BasicTextFormattedInlineBackground.html", false, 15000)]
        [InlineData("BasicTextFormattedInlineBackground.html", true, 9500)]
        public async Task ShouldConvertHtmlToPngBufferOptions(string testFileName, bool omitBackground, int allowedPixelDiff)
        {
            var sourceHtmlFilePath = $"../../../TestInput/{testFileName}";
            var actualFilePath = Path.Combine(Path.GetTempPath(), $"ActualConvertHtmlToPng{testFileName}.{omitBackground}.png");
            var expectReferenceFilePath = $"../../../ExpectedTestOutcome/ExpectedConvertHtmlToPng{testFileName}.{omitBackground}.png";

            if (File.Exists(actualFilePath))
            {
                File.Delete(actualFilePath);
            }

            await using (var chromiumRenderer = await Renderer.CreateAsync())
            {
                ScreenshotOptions screenshotOptions = new ScreenshotOptions
                {
                    OmitBackground = omitBackground
                };

                var fileContent = await File.ReadAllTextAsync(sourceHtmlFilePath);
                var pngData = await chromiumRenderer.ConvertHtmlStringToPngData(fileContent, screenshotOptions);
                // File.Copy(actualFilePath, expectReferenceFilePath, true);
                await File.WriteAllBytesAsync(actualFilePath, pngData);
                DocumentAsserter.AssertImageIsEqual(actualFilePath, expectReferenceFilePath, allowedPixelDiff);
            }

            File.Delete(actualFilePath);
            await ChromiumProcessDisposedAsserter.AssertNoChromiumProcessIsRunning();
        }

        [Fact]
        public async Task ShouldDisposeGraceful()
        {
            var initialChromiumTasks = ChromiumProcessDisposedAsserter.CountChromiumTasks();

            await using (var chromiumRenderer = new Renderer())
            {
                Assert.Null(chromiumRenderer.Browser);
            }
            var afterDisposeChromiumTasks = ChromiumProcessDisposedAsserter.CountChromiumTasks();
            Assert.Equal(afterDisposeChromiumTasks, initialChromiumTasks);
        }

        [Theory]
        [InlineData("BasicTextFormatted.html")]
        public async Task ShouldConvertHtmlToPngNoSandbox(string testFileName)
        {
            var sourceHtmlFilePath = $"../../../TestInput/{testFileName}";
            var actualFilePath = Path.Combine(Path.GetTempPath(), $"ActualConvertHtmlToPng{testFileName}.png");
            var expectReferenceFilePath = $"../../../ExpectedTestOutcome/ExpectedConvertHtmlToPng{testFileName}.png";

            if (File.Exists(actualFilePath))
            {
                File.Delete(actualFilePath);
            }

            using (var chromiumRenderer = await Renderer.CreateAsync(new BrowserFetcher(), "--no-sandbox"))
            {
                await chromiumRenderer.ConvertHtmlToPng(sourceHtmlFilePath, actualFilePath);

                DocumentAsserter.AssertImageIsEqual(actualFilePath, expectReferenceFilePath, 9000);
            }

            File.Delete(actualFilePath);
            await ChromiumProcessDisposedAsserter.AssertNoChromiumProcessIsRunning();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Rasterize?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}