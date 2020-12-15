using Codeuctivity;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Xunit;

namespace PuppeteerSharp.RendererTests
{
    internal static class DocumentAsserter
    {
        internal static void AssertImageIsEqual(string actualImagePath, string expectImageFilePath)
        {
            var actualFullPath = Path.GetFullPath(actualImagePath);
            var expectFullPath = Path.GetFullPath(expectImageFilePath);

            Assert.True(File.Exists(actualFullPath), $"actualImagePath not found {actualFullPath}");
            Assert.True(File.Exists(expectFullPath), $"ExpectReferenceImagePath not found \n{expectFullPath}\n copy over \n{actualFullPath}\n if this is a new test case.");

            if (ImageSharpCompare.ImageAreEqual(actualFullPath, expectFullPath))
            {
                return;
            }

            var osSpezificDiffFileSuffix = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" : "win";

            var allowedDiffImage = $"{expectFullPath}.diff.{osSpezificDiffFileSuffix}.png";

            if (File.Exists(allowedDiffImage))
            {
                var result = ImageSharpCompare.CalcDiff(actualFullPath, expectFullPath, allowedDiffImage);

                if (result.AbsoluteError == 0)
                {
                    return;
                }
            }

            var newDiffImage = $"{actualFullPath}.diff.png";
            using (var fileStreamDifferenceMask = File.Create(newDiffImage))
            using (var maskImage = ImageSharpCompare.CalcDiffMaskImage(actualFullPath, expectFullPath))
            {
                SixLabors.ImageSharp.ImageExtensions.SaveAsPng(maskImage, fileStreamDifferenceMask);
            }

            var base64fyedActualImage = Convert.ToBase64String(File.ReadAllBytes(actualFullPath));
            var base64fyedImageDiff = Convert.ToBase64String(File.ReadAllBytes(newDiffImage));
            Assert.True(ImageSharpCompare.ImageAreEqual(actualFullPath, expectFullPath), $"Expected {expectFullPath}\ndiffers to actual {actualFullPath}\n {base64fyedActualImage}\n \n Diff is {newDiffImage} \n {base64fyedImageDiff}\n");
        }
    }
}