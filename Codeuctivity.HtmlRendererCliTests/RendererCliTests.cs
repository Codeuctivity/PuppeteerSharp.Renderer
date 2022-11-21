using Codeuctivity.HtmlRendererCli;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Codeuctivity.HtmlRendererCliTests
{
    public class RendererCliTests
    {
        [Fact]
        public void VersionShouldBeProcessed()
        {
            var success = Version.TryParse(Program.Version.ToString(), out _);
            Assert.True(success);
        }

        [Fact]
        public async Task ConversionShouldWork()
        {
            var testFileName = "BasicTextFormated.html";
            var sourceHtmlFilePath = $"../../../TestInput/{testFileName}";
            var actualFilePath = Path.Combine(Path.GetTempPath(), $"Actual{testFileName}.pdf");

            await Program.Main(new[] { sourceHtmlFilePath, actualFilePath });

            Assert.True(File.Exists(actualFilePath));
        }

        [FactRunableOnWindows]
        public void PublishedSelfContainedBinaryShouldWork()
        {
            var testFileName = "BasicTextFormated.html";
            var sourceHtmlFilePath = Path.GetFullPath($"../../../TestInput/{testFileName}");
            var actualFilePath = Path.Combine(Path.GetTempPath(), $"Actual{testFileName}.pdf");

            if (File.Exists(actualFilePath))
            {
                File.Delete(actualFilePath);
            }

            var acutualWindowsBinary = DotnetPublishFolderProfileWindows("Codeuctivity.HtmlRendererCli");

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = acutualWindowsBinary,
                    Arguments = $" {sourceHtmlFilePath} {actualFilePath}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            var isExited = process.WaitForExit(60000);

            if (!isExited)
            {
                process.Kill();
                // run on windows: Stop-Process -Name "chrome"
            }

            Assert.True(isExited);

            Assert.True(File.Exists(actualFilePath));
        }

        private static string DotnetPublishFolderProfileWindows(string projectName)
        {
            var absolutePath = Path.GetFullPath("../../../../" + projectName);
            var expectedBinaryPath = Path.Combine(absolutePath, $"bin/Release/net7.0/publish/FolderProfileWindows/{projectName}.exe");

            if (File.Exists(expectedBinaryPath))
            {
                File.Delete(expectedBinaryPath);
            }

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"publish {absolutePath} --configuration Release -f net7.0 -p:PublishProfile=FolderProfileWindows",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();

            var outputResult = GetStreamOutput(process.StandardOutput);
            var errorResult = GetStreamOutput(process.StandardError);
            Assert.True(process.WaitForExit(20000));

            Assert.Empty(errorResult);

            Assert.True(File.Exists(expectedBinaryPath), outputResult);

            return expectedBinaryPath;
        }

        private static string GetStreamOutput(StreamReader stream)
        {
            var outputReadTask = Task.Run(() => stream.ReadToEnd());

            return outputReadTask.Result;
        }
    }
}