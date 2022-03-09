using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Codeuctivity.HtmlRendererTests.Infrastrukture
{
    public static class ChromiumProcessDisposedAsserter
    {
        public static async Task AssertNoChromeProcessIsRunning()
        {
            for (var i = 0; i < 20 && CountChromiumTasks() > 0; i++)
            {
                await Task.Delay(200);
            }
            Assert.Equal(0, CountChromiumTasks());
        }

        public static int CountChromiumTasks()
        {
            var processes = Process.GetProcesses().Where(process => process.ProcessName.Contains("chrome")).ToList();
            return processes.Count;
        }
    }
}