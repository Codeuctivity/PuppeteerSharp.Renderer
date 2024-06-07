using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Codeuctivity.HtmlRendererTests.Infrastructure
{
    public static class ChromiumProcessDisposedAsserter
    {
        private const int maxExpectedBackgroundProcesses = 5;

        public static async Task AssertNoChromiumProcessIsRunningExceptExpectedOrphanedBackgroundChromeProcesses()
        {
            for (var i = 0; i < 20 && CountChromiumTasks() > maxExpectedBackgroundProcesses; i++)
            {
                await Task.Delay(200);
            }
            Assert.True(CountChromiumTasks() <= maxExpectedBackgroundProcesses);
        }

        public static int CountChromiumTasks()
        {
            var processes = Process.GetProcesses().Where(process => process.ProcessName.Contains("chrome")).ToList();
            return processes.Count;
        }
    }
}