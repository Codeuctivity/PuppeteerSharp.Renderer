using Codeuctivity.HtmlRenderer;
using PuppeteerSharp;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Codeuctivity.HtmlRendererCli
{
    public static class Program
    {
        public static readonly Assembly Reference = typeof(Renderer).Assembly;
        public static readonly Version Version = Reference.GetName().Version;
        private static int _lastProgressValue;

        public static async Task<int> Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: PuppeteerSharp.RendererCli <sourceHtmlFilePath> <destinatioPdfFilePath>");
                return 1;
            }

            var inputPathDocx = args[0];
            var outputPathHtml = args[1];

            if (!File.Exists(inputPathDocx))
            {
                Console.WriteLine($"Could not find source {inputPathDocx}.");
                return 1;
            }

            if (File.Exists(outputPathHtml))
            {
                Console.WriteLine($"Destination {outputPathHtml} already exists.");
                return 1;
            }

            Console.WriteLine($"Converting {inputPathDocx} to {outputPathHtml} using PuppeteerSharp.Renderer {Version}");
            BrowserFetcherOptions options = new BrowserFetcherOptions();
            options.Path = Path.GetTempPath();
            using var browserFetcher = new BrowserFetcher(options);
            Console.WriteLine($"Fetching chromium from web, to {browserFetcher.CacheDir} .... ");
            browserFetcher.DownloadProgressChanged += BrowserFetcher_DownloadProgressChanged;

            await using (var chromiumRenderer = await Renderer.CreateAsync(browserFetcher, string.Empty))
            {
                await chromiumRenderer.ConvertHtmlToPdf(inputPathDocx, outputPathHtml);
            }
            return 0;
        }

        private static void BrowserFetcher_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage != _lastProgressValue)
            {
                _lastProgressValue = e.ProgressPercentage;
                Console.WriteLine($" {e.ProgressPercentage}%");
            }
        }
    }
}