using Codeuctivity.PdfjsSharp;
using Codeuctivity.PuppeteerSharp;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.RendererTests
{
    public class RendererTests
    {
        private readonly ITestOutputHelper output;

        public RendererTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("BasicTextFormated.html")]
        public async Task ShouldConvertHtmlToPdf(string testFileName)
        {
            var sourceHtmlFilePath = $"../../../TestInput/{testFileName}";
            var actualFilePath = Path.Combine(Path.GetTempPath(), $"Actual{testFileName}.pdf");
            var expectReferenceFilePath = $"../../../ExpectedTestOutcome/ExpectedFromHtml{testFileName}.png";

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
            DocumentAsserter.AssertImageIsEqual(actualImages.Single(), expectReferenceFilePath);
        }

        [Theory]
        [InlineData("BasicTextFormated.html")]
        public async Task ShouldConvertHtmlToPng(string testFileName)
        {
            var sourceHtmlFilePath = $"../../../TestInput/{testFileName}";
            var actualFilePath = Path.Combine(Path.GetTempPath(), $"Actual{testFileName}.png");
            var expectReferenceFilePath = $"../../../ExpectedTestOutcome/Expected{testFileName}.png";

            if (File.Exists(actualFilePath))
            {
                File.Delete(actualFilePath);
            }

            await using var chromiumRenderer = await Renderer.CreateAsync();

            await chromiumRenderer.ConvertHtmlToPng(sourceHtmlFilePath, actualFilePath);

            DocumentAsserter.AssertImageIsEqual(actualFilePath, expectReferenceFilePath);
        }
    }
}