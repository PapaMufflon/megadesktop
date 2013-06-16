using System;
using System.Diagnostics;

namespace MegaDesktop.Tests
{
    public static class AsyncTestsHelper
    {
        public static void WaitFor(Func<bool> predicate)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (!predicate() && stopwatch.Elapsed < TimeSpan.FromSeconds(5))
                System.Threading.Thread.Sleep(40);
        }
    }
}