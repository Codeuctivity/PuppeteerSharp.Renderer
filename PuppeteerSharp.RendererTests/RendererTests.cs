using Codeuctivity.PdfjsSharp;
using Codeuctivity.PuppeteerSharp;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.RendererTests
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

            await using var chromiumRenderer = await Renderer.CreateAsync();
            await chromiumRenderer.ConvertHtmlToPdf(sourceHtmlFilePath, actualFilePath);

            var actualImagePathDirectory = Path.Combine(Path.GetTempPath(), testFileName);

            using var rasterize = new Rasterizer();
            var actualImages = await rasterize.ConvertToPngAsync(actualFilePath, actualImagePathDirectory);

            Assert.Single(actualImages);
            DocumentAsserter.AssertImageIsEqual(actualImages.Single(), expectReferenceFilePath, 50);
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

            await using var chromiumRenderer = await Renderer.CreateAsync();

            await chromiumRenderer.ConvertHtmlToPng(sourceHtmlFilePath, actualFilePath);

            DocumentAsserter.AssertImageIsEqual(actualFilePath, expectReferenceFilePath, 50);
        }

        [Fact]
        public async Task ShouldDisposeGracefull()
        {
            await using (var chromiumRenderer = new Renderer())
            {
                Assert.Null(chromiumRenderer.BrowserFetcher);
            }
        }
    }
}