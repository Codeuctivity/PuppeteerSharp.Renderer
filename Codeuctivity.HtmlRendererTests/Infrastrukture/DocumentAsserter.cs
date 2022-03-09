using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace Codeuctivity.HtmlRendererTests.Infrastrukture
{
    internal static class DocumentAsserter
    {
        internal static void AssertImageIsEqual(string actualImagePath, string expectImageFilePath, int allowedPixelErrorCount)
        {
            var actualFullPath = Path.GetFullPath(actualImagePath);
            var expectFullPath = Path.GetFullPath(expectImageFilePath);

            Assert.True(File.Exists(actualFullPath), $"actualImagePath not found {actualFullPath}");
            Assert.True(File.Exists(expectFullPath), $"ExpectReferenceImagePath not found \n{expectFullPath}\n copy over \n{actualFullPath}\n if this is a new test case.");
            var base64fyedActualImage = Convert.ToBase64String(File.ReadAllBytes(actualFullPath));

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
            var base64fyedImageDiff = Convert.ToBase64String(File.ReadAllBytes(newDiffImage));

            if (File.Exists(allowedDiffImage))
            {
                var resultWithAllowedDiff = ImageSharpCompare.ImageSharpCompare.CalcDiff(actualFullPath, expectFullPath, allowedDiffImage);

                Assert.True(resultWithAllowedDiff.PixelErrorCount <= allowedPixelErrorCount, $"Expected PixelErrorCount beyond {allowedPixelErrorCount} but was {resultWithAllowedDiff.PixelErrorCount}\nExpected {expectFullPath}\ndiffers to actual {actualFullPath}\n {base64fyedActualImage}\n \n Diff is {newDiffImage} \n {base64fyedImageDiff}\n");
                return;
            }

            using (var fileStreamDifferenceMask = File.Create(newDiffImage))
            using (var maskImage = ImageSharpCompare.ImageSharpCompare.CalcDiffMaskImage(actualFullPath, expectFullPath))
            {
                SixLabors.ImageSharp.ImageExtensions.SaveAsPng(maskImage, fileStreamDifferenceMask);
            }

            var result = ImageSharpCompare.ImageSharpCompare.CalcDiff(actualFullPath, expectFullPath);

            Assert.True(result.PixelErrorCount <= allowedPixelErrorCount, $"Expected PixelErrorCount beyond {allowedPixelErrorCount} but was {result.PixelErrorCount}\nExpected {expectFullPath}\ndiffers to actual {actualFullPath}\n {base64fyedActualImage}\n \n Diff is {newDiffImage} \n {base64fyedImageDiff}\nReplace {actualFullPath} with the new value or store the diff as {allowedDiffImage}.");
        }
    }
}