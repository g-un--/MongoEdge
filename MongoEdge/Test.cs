using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoEdge
{
    public static class Test
    {
        public static Func<Task> WithStopwatch(this Func<Task> task, string name)
        {
            return () =>
            {
                var stopwatch = Stopwatch.StartNew();
                return task().ContinueWith(_ =>
                    {
                        stopwatch.Stop();
                        Console.WriteLine(string.Format("test: {0}. ms: {1}", name, stopwatch.ElapsedMilliseconds));
                    });
            };
        }
    }
}
