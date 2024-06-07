using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace Codeuctivity.HtmlRendererTests.Infrastructure
{
    internal static class DocumentAsserter
    {
        private const string TestOutputFirectory = "../../../../TestResult";

        internal static void AssertImageIsEqual(string actualImagePath, string expectImageFilePath, int allowedPixelErrorCount)
        {
            var actualFullPath = Path.GetFullPath(actualImagePath);
            var expectFullPath = Path.GetFullPath(expectImageFilePath);

            Assert.True(File.Exists(actualFullPath), $"actualImagePath not found {actualFullPath}");
            // File.Copy(actualFullPath, expectFullPath, true);
            Assert.True(File.Exists(expectFullPath), $"ExpectReferenceImagePath not found \n{expectFullPath}\n copy over \n{actualFullPath}\n if this is a new test case.");

            if (ImageSharpCompare.ImageSharpCompare.ImagesAreEqual(actualFullPath, expectFullPath))
            {
                return;
            }

            var osSpecificDiffFileSuffix = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" : "win";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                osSpecificDiffFileSuffix = "osx";
            }

            var allowedDiffImage = $"{expectFullPath}.diff.{osSpecificDiffFileSuffix}.png";
            var newDiffImage = $"{actualFullPath}.diff.png";
            using (var fileStreamDifferenceMask = File.Create(newDiffImage))
            using (var maskImage = ImageSharpCompare.ImageSharpCompare.CalcDiffMaskImage(actualFullPath, expectFullPath))
            {
                SixLabors.ImageSharp.ImageExtensions.SaveAsPng(maskImage, fileStreamDifferenceMask);
            }

            if (File.Exists(allowedDiffImage))
            {
                var resultWithAllowedDiff = ImageSharpCompare.ImageSharpCompare.CalcDiff(actualFullPath, expectFullPath, allowedDiffImage);

                if (allowedPixelErrorCount < resultWithAllowedDiff.PixelErrorCount)
                {
                    CopyToTestOutput(actualImagePath);
                    CopyToTestOutput(expectImageFilePath);
                    CopyToTestOutput(newDiffImage);
                }

                Assert.True(resultWithAllowedDiff.PixelErrorCount <= allowedPixelErrorCount, $"Expected PixelErrorCount beyond {allowedPixelErrorCount} but was {resultWithAllowedDiff.PixelErrorCount}\nExpected {expectFullPath}\ndiffers to actual {actualFullPath}\n Diff is {newDiffImage}");
                return;
            }

            using (var fileStreamDifferenceMask = File.Create(newDiffImage))
            using (var maskImage = ImageSharpCompare.ImageSharpCompare.CalcDiffMaskImage(actualFullPath, expectFullPath))
            {
                SixLabors.ImageSharp.ImageExtensions.SaveAsPng(maskImage, fileStreamDifferenceMask);
            }

            var result = ImageSharpCompare.ImageSharpCompare.CalcDiff(actualFullPath, expectFullPath);

            if (allowedPixelErrorCount < result.PixelErrorCount)
            {
                // File.Copy(newDiffImage, allowedDiffImage);
                CopyToTestOutput(newDiffImage);
                CopyToTestOutput(actualImagePath);
            }

            Assert.True(result.PixelErrorCount <= allowedPixelErrorCount, $"Expected PixelErrorCount beyond {allowedPixelErrorCount} but was {result.PixelErrorCount}\nExpected {expectFullPath}\ndiffers to actual {actualFullPath}\n Diff is {newDiffImage}\nReplace {actualFullPath} with the new value or store the diff as {allowedDiffImage}.");
        }

        private static void CopyToTestOutput(string testOutputFile)
        {
            if (string.IsNullOrEmpty(testOutputFile))
            {
                throw new System.ArgumentException($"'{nameof(testOutputFile)}' cannot be null or empty.", nameof(testOutputFile));
            }

            if (!Directory.Exists(TestOutputFirectory))
            {
                Directory.CreateDirectory(TestOutputFirectory);
            }

            File.Copy(testOutputFile, Path.Combine(TestOutputFirectory, Path.GetFileName(testOutputFile)), true);
        }
    }
}