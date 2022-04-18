using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace Codeuctivity.HtmlRendererTests.Infrastructure
{
    internal static class DocumentAsserter
    {
        private const string TestOutput = "../../../TestResult";

        internal static void AssertImageIsEqual(string actualImagePath, string expectImageFilePath, int allowedPixelErrorCount)
        {
            var actualFullPath = Path.GetFullPath(actualImagePath);
            var expectFullPath = Path.GetFullPath(expectImageFilePath);

            Assert.True(File.Exists(actualFullPath), $"actualImagePath not found {actualFullPath}");
            Assert.True(File.Exists(expectFullPath), $"ExpectReferenceImagePath not found \n{expectFullPath}\n copy over \n{actualFullPath}\n if this is a new test case.");

            if (ImageSharpCompare.ImageSharpCompare.ImagesAreEqual(actualFullPath, expectFullPath))
            {
                return;
            }

            var osSpecificDiffFileSuffix = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" : "win";

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
                    CopyToTestOutput(actualImagePath, newDiffImage);
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
                CopyToTestOutput(actualImagePath, newDiffImage);
            }

            Assert.True(result.PixelErrorCount <= allowedPixelErrorCount, $"Expected PixelErrorCount beyond {allowedPixelErrorCount} but was {result.PixelErrorCount}\nExpected {expectFullPath}\ndiffers to actual {actualFullPath}\n Diff is {newDiffImage}\nReplace {actualFullPath} with the new value or store the diff as {allowedDiffImage}.");
        }

        private static void CopyToTestOutput(string actualImagePath, string newDiffImage)
        {
            if (!Directory.Exists(TestOutput))
                Directory.CreateDirectory(TestOutput);

            File.Copy(newDiffImage, Path.Combine(TestOutput, Path.GetFileName(newDiffImage)));
            File.Copy(actualImagePath, Path.Combine(TestOutput, Path.GetFileName(actualImagePath)));
        }
    }
}