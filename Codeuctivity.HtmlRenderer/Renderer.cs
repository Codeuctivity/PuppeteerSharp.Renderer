using PuppeteerSharp;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Codeuctivity.HtmlRenderer
{
    /// <summary>
    /// Renders HTML files
    /// </summary>
    public class Renderer : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="customChromiumArgs"></param>
        public Renderer(string? customChromiumArgs)
        {
            if (customChromiumArgs == null)
            {
                LaunchOptions = SystemSpecificConfig();
            }
            else
            {
                LaunchOptions = new LaunchOptions() { Args = new[] { customChromiumArgs } };
            }
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="launchOptions"></param>
        public Renderer(LaunchOptions? launchOptions = null)
        {
            if (launchOptions == null)
            {
                LaunchOptions = SystemSpecificConfig();
            }
            else
            {
                LaunchOptions = launchOptions;
            }
        }

        private IBrowser Browser { get; set; } = default!;
        private int LastProgressValue { get; set; }

        /// <summary>
        /// Browser fetcher - used to get chromium bins
        /// </summary>
        public BrowserFetcher BrowserFetcher { get; private set; } = default!;

        private LaunchOptions LaunchOptions { get; }

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
        /// Call CreateAsync before using ConvertHtmlTo*, accepts custom BrowserFetcher and custom chromium launch options
        /// </summary>
        /// <param name="browserFetcher"></param>
        /// <param name="chromiumArguments">Adds custom arguments to chromium</param>
        /// <returns></returns>
        public static Task<Renderer> CreateAsync(BrowserFetcher browserFetcher, string chromiumArguments)
        {
            return CreateAsync(browserFetcher, new LaunchOptions() { Args = new[] { chromiumArguments } });
        }

        /// <summary>
        /// Call CreateAsync before using ConvertHtmlTo*, accepts custom BrowserFetcher and custom chromium launch options
        /// </summary>
        /// <param name="browserFetcher"></param>
        /// <param name="launchOptions">Adds launch options to puppeteer</param>
        /// <returns></returns>
        public static Task<Renderer> CreateAsync(BrowserFetcher browserFetcher, LaunchOptions? launchOptions = null)
        {
            var html2Pdf = new Renderer(launchOptions);
            return html2Pdf.InitializeAsync(browserFetcher);
        }

        private async Task<Renderer> InitializeAsync(BrowserFetcher browserFetcher)
        {
            BrowserFetcher = browserFetcher;
            BrowserFetcher.DownloadProgressChanged += DownloadProgressChanged;

            _ = await BrowserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision ?? string.Empty).ConfigureAwait(false);
            Browser = await Puppeteer.LaunchAsync(LaunchOptions).ConfigureAwait(false);
            return this;
        }

        private LaunchOptions SystemSpecificConfig()
        {
            if (IsRunningOnWslOrAzure() || IsRunningOnAzureLinux() || IsRunningLinux())
            {
                return new LaunchOptions { Headless = true, Args = new string[] { "--no-sandbox" } };
            }
            return new LaunchOptions();
        }

        private static bool IsRunningLinux()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return false;
            }

            var version = File.ReadAllText("/proc/version");
            var IsLinux = version.IndexOf("Linux", StringComparison.OrdinalIgnoreCase) >= 0;

            return IsLinux;
        }

        private static bool IsRunningOnAzureLinux()
        {
            var websiteSku = Environment.GetEnvironmentVariable("WEBSITE_SKU");

            if (string.IsNullOrEmpty(websiteSku))
            {
                return false;
            }

            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && websiteSku.IndexOf("Linux", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsRunningOnWslOrAzure()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return false;
            }

            var version = File.ReadAllText("/proc/version");
            var IsWsl = version.IndexOf("Microsoft", StringComparison.OrdinalIgnoreCase) >= 0;
            var IsAzure = version.IndexOf("azure", StringComparison.OrdinalIgnoreCase) >= 0;

            return IsWsl || IsAzure;
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
            await using var page = (await Browser.NewPageAsync().ConfigureAwait(false));
            await page.GoToAsync($"file://{absolutePath}").ConfigureAwait(false);
            // Wait for fonts to be loaded. Omitting this might result in no text rendered in PDF.
            await page.EvaluateExpressionHandleAsync("document.fonts.ready");
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
            await using var page = (await Browser.NewPageAsync().ConfigureAwait(false));
            await page.GoToAsync($"file://{absolutePath}").ConfigureAwait(false);
            // Wait for fonts to be loaded. Omitting this might result in no text the screenshot.
            await page.EvaluateExpressionHandleAsync("document.fonts.ready");
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
            await DisposeAsyncCore().ConfigureAwait(false);

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
                await Browser.CloseAsync().ConfigureAwait(false);
                await Browser.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}