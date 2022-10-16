using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SandyBridge.Infrastructure
{
    public static class Run
    {
        /// <summary>
        /// Run an expression and track its duration and success.
        /// </summary>
        /// <typeparam name="T">Return value</typeparam>
        /// <param name="func">Expression to execute</param>
        /// <param name="action">Called with the duration and a flag indicating success</param>
        /// <returns>The value of <paramref name="func"/></returns>
        [DebuggerStepThrough]
        public static async Task<T> TimedAsync<T>(Func<Task<T>> func, Action<bool, TimeSpan> action)
        {
            var success = false;
            T result;

            var stopwatch = Stopwatch.StartNew();

            try
            {
                result = await func();
                success = true;
            }
            finally
            {
                stopwatch.Stop();
                action(success, stopwatch.Elapsed);
            }

            return result;
        }
    }
}
