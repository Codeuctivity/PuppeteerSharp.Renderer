using Codeuctivity.HtmlRenderer;
using Codeuctivity.HtmlRendererTests.Infrastructure;
using Codeuctivity.PdfjsSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Codeuctivity.HtmlRendererTests
{
    public class RendererTests
    {
        [Theory]
        [InlineData("BasicTextFormated.html")]
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

                using var rasterize = new Rasterizer();

                if (!IsRunningOnWslOrAzure())
                {
                    var actualImages = await rasterize.ConvertToPngAsync(actualFilePath, actualImagePathDirectory);
                    Assert.Single(actualImages);
                    DocumentAsserter.AssertImageIsEqual(actualImages.Single(), expectReferenceFilePath, 2000);
                }
            }
            await ChromiumProcessDisposedAsserter.AssertNoChromeProcessIsRunning();
        }

        private static bool IsRunningOnWslOrAzure()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return false;
            }

            var version = File.ReadAllText("/proc/version");
            var IsAzure = version.IndexOf("Microsoft", StringComparison.OrdinalIgnoreCase) >= 0;
            var IsWsl = version.IndexOf("azure", StringComparison.OrdinalIgnoreCase) >= 0;

            return IsWsl || IsAzure;
        }

        [Theory]
        [InlineData("BasicTextFormated.html")]
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

                DocumentAsserter.AssertImageIsEqual(actualFilePath, expectReferenceFilePath, 7000);
            }

            await ChromiumProcessDisposedAsserter.AssertNoChromeProcessIsRunning();
        }

        [Fact]
        public async Task ShouldDisposeGracefull()
        {
            var initialChromiumTasks = ChromiumProcessDisposedAsserter.CountChromiumTasks();

            await using (var chromiumRenderer = new Renderer())
            {
                Assert.Null(chromiumRenderer.BrowserFetcher);
            }
            var afterDisposeChromiumTasks = ChromiumProcessDisposedAsserter.CountChromiumTasks();
            Assert.Equal(afterDisposeChromiumTasks, initialChromiumTasks);
        }
    }
}