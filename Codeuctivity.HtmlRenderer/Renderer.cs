using PuppeteerSharp;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Codeuctivity.HtmlRenderer
{
    /// <summary>
    /// Renders HTML files
    /// </summary>
    public class Renderer : IAsyncDisposable, IDisposable
    {
        private Browser Browser { get; set; } = default!;
        private int LastProgressValue { get; set; }

        /// <summary>
        /// Browser fetcher - used to get chromium bins
        /// </summary>
        public BrowserFetcher BrowserFetcher { get; private set; } = default!;

        /// <summary>
        /// Call CreateAsync before using ConvertHtmlTo*
        /// </summary>
        /// <returns>Initialized renderer</returns>
        public static Task<Renderer> CreateAsync()
        {
            var html2Pdf = new Renderer();
            return html2Pdf.InitializeAsync(new BrowserFetcher());
        }

        /// <summary>
        /// Call CreateAsync before using ConvertHtmlTo*, accepts custom BrowserFetcher
        /// </summary>
        /// <returns>Initialized renderer</returns>
        public static Task<Renderer> CreateAsync(BrowserFetcher browserFetcher)
        {
            var html2Pdf = new Renderer();
            return html2Pdf.InitializeAsync(browserFetcher);
        }

        private async Task<Renderer> InitializeAsync(BrowserFetcher browserFetcher)
        {
            BrowserFetcher = browserFetcher;
            BrowserFetcher.DownloadProgressChanged += DownloadProgressChanged;

            await BrowserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            Browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            return this;
        }

        /// <summary>
        /// Converts a HTML file to a PDF
        /// </summary>
        /// <param name="sourceHtmlFilePath"></param>
        /// <param name="destinationPdfFilePath"></param>
        public async Task ConvertHtmlToPdf(string sourceHtmlFilePath, string destinationPdfFilePath)
        {
            if (!File.Exists(sourceHtmlFilePath))
            {
                throw new FileNotFoundException(sourceHtmlFilePath);
            }

            var absolutePath = Path.GetFullPath(sourceHtmlFilePath);
            await using var page = await Browser.NewPageAsync().ConfigureAwait(false);
            await page.GoToAsync($"file://{absolutePath}").ConfigureAwait(false);
            await page.PdfAsync(destinationPdfFilePath).ConfigureAwait(false);
        }

        /// <summary>
        /// Converts a HTML file to a PNG
        /// </summary>
        /// <param name="sourceHtmlFilePath"></param>
        /// <param name="destinationPngFilePath"></param>
        public async Task ConvertHtmlToPng(string sourceHtmlFilePath, string destinationPngFilePath)
        {
            if (!File.Exists(sourceHtmlFilePath))
            {
                throw new FileNotFoundException(sourceHtmlFilePath);
            }

            var absolutePath = Path.GetFullPath(sourceHtmlFilePath);
            await using var page = await Browser.NewPageAsync().ConfigureAwait(false);
            await page.GoToAsync($"file://{absolutePath}").ConfigureAwait(false);
            await page.ScreenshotAsync(destinationPngFilePath, new ScreenshotOptions { FullPage = true }).ConfigureAwait(false);
        }

        private void DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (LastProgressValue != e.ProgressPercentage)
            {
                LastProgressValue = e.ProgressPercentage;
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// DisposeAsync
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Browser?.Dispose();
            }
        }

        /// <summary>
        /// DisposeAsync
        /// </summary>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (Browser is not null)
            {
                await Browser.CloseAsync();
                await Browser.DisposeAsync();
            }
        }
    }
}