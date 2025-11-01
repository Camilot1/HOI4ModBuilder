using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HOI4ModBuilder.src.utils
{
    public class ParallelUtils
    {
        public static void Execute(IEnumerable<(string, Action)> entries)
        {
            var anyFailed = new bool[1];
            Parallel.ForEach(entries, entry =>
            {
                try
                {
                    if (entry.Item1 == null)
                        entry.Item2?.Invoke();
                    else
                    {
                        var stopwatch = Stopwatch.StartNew();
                        entry.Item2?.Invoke();
                        Logger.Log("    Executed action: " + entry.Item1 + ": " + stopwatch.ElapsedMilliseconds + " ms");
                    }
                }
                catch (Exception ex)
                {
                    anyFailed[0] = true;
                    if (entry.Item1 != null)
                        Logger.Log("    Exception raised at action: " + entry.Item1);
                    Logger.LogException(ex);
                }
            });
            if (anyFailed[0])
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_SAVING_PROCESS_ABORTED));
        }

        public static void Execute(IEnumerable<(string, Action)> entries, int checkDelayMS, Action<int, int> onTaskEnd)
        {
            int total = entries.Count();
            int completed = 0;
            int failed = 0;

            int cachedDisplay = 0;
            onTaskEnd?.Invoke(cachedDisplay, total);

            foreach (var entry in entries)
                Task.Run(() =>
                {
                    try
                    {
                        if (entry.Item1 == null)
                            entry.Item2?.Invoke();
                        else
                        {
                            var stopwatch = Stopwatch.StartNew();
                            entry.Item2?.Invoke();
                            Logger.Log("    Executed action: " + entry.Item1 + ": " + stopwatch.ElapsedMilliseconds + " ms");
                        }
                        Interlocked.Increment(ref completed);
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref failed);
                        if (entry.Item1 != null)
                            Logger.Log("    Exception raised at action: " + entry.Item1);
                        Logger.LogException(ex);
                    }
                });

            do
            {
                int sumCount = completed + failed;
                if (sumCount != cachedDisplay)
                {
                    onTaskEnd?.Invoke(sumCount, total);
                    cachedDisplay = sumCount;
                }
                Thread.Sleep(checkDelayMS);
            } while (completed + failed < total);

            if (failed != 0)
                throw new Exception(GuiLocManager.GetLoc(EnumLocKey.EXCEPTION_SAVING_PROCESS_ABORTED));
        }
    }
}
