using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PingAI.DialogManagementService.Application.Utils
{
    // TODO: use logger for logging
    public static class PerformanceLogger
    {
        public static async Task Monitor(Func<Task> run, string name)
        {
            var sw = new Stopwatch();
            sw.Start();
            await run();
            sw.Stop();
            Console.WriteLine($"Executing {name} took {sw.Elapsed.TotalSeconds} seconds.");
        }
        
        public static async Task<T> Monitor<T>(Func<Task<T>> run, string name)
        {
            var sw = new Stopwatch();
            sw.Start();
            var result = await run();
            sw.Stop();
            Console.WriteLine($"Executing {name} took {sw.Elapsed.TotalSeconds} seconds.");
            return result;
        }
    }
}
