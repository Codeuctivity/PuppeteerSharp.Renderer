using System.Runtime.InteropServices;
using Xunit;

namespace Codeuctivity.HtmlRendererCliTests
{
    internal class FactRunnableOnWindowsAttribute : FactAttribute
    {
        public FactRunnableOnWindowsAttribute()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return;
            }

            Skip = "Test will only run on windows.";
        }
    }
}