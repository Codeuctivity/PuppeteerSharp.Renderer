using System.Runtime.InteropServices;
using Xunit;

namespace Codeuctivity.HtmlRendererCliTests
{
    internal class FactRunableOnWindowsAttribute : FactAttribute
    {
        public FactRunableOnWindowsAttribute()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            Skip = "Test will only run on windows.";
        }
    }
}