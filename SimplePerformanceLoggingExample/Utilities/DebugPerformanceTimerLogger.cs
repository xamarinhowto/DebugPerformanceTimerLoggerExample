using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DarkIce.Toolkit.Core.Utilities
{
    public class DebugPerformanceTimerLogger : IDisposable
    {
        private readonly string _message;

        private readonly Stopwatch _timer;

        private static List<MutableKeyValuePair<string, long>> _log = new List<MutableKeyValuePair<string, long>>();

        public DebugPerformanceTimerLogger([CallerMemberName] string message = null, [CallerFilePath] string sourceFile = null)
        {
#if DEBUG
            try
            {
                var fileName = Path.GetFileNameWithoutExtension(sourceFile.Replace("\\", "/"));

                _message = $"{fileName}: {message}".Trim();
                _timer = new Stopwatch();
                _timer.Start();
                Debug.WriteLine($"{DateTime.Now:hh:mm:ss.ffff} START: {_message}");

                _log.Add(new MutableKeyValuePair<string, long>(_message, 0));

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
#endif
        }

        public void Dispose()
        {
#if DEBUG
            try
            {
                _timer.Stop();
                var ms = _timer.ElapsedMilliseconds;

                Debug.WriteLine($"{DateTime.Now:hh:mm:ss.ffff} END  : {_message} ({ms}ms)");

                _log.LastOrDefault(x => x.Key == _message).Value = ms;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
#endif
        }

        [Conditional("DEBUG")]
        public static void GetSummary()
        {
            Debug.WriteLine("");
            Debug.WriteLine("PERFORMANCE SUMMARY");
            Debug.WriteLine("");

            foreach (var entry in _log)
            {
                Debug.WriteLine($"CALL: {entry.Key} EXECUTION TIME: {entry.Value}ms");
            }

            Debug.WriteLine("");
            Debug.WriteLine("------------------------------------");
            Debug.WriteLine($" TOTAL TIME: {_log.Sum(x => x.Value)} ms");
            Debug.WriteLine("------------------------------------");

            _log.Clear();
        }

        private class MutableKeyValuePair<KeyType, ValueType>
        {
            public KeyType Key { get; set; }
            public ValueType Value { get; set; }

            public MutableKeyValuePair() { }

            public MutableKeyValuePair(KeyType key, ValueType val)
            {
                Key = key;
                Value = val;
            }
        }
    }
}
