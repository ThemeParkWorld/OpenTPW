using System;
using System.Diagnostics;

namespace Quincy.DebugUtils
{
    public class LogEntry
    {
        public DateTime timestamp;
        public StackTrace stackTrace;
        public string str;
        public Logging.Severity severity;

        public LogEntry(DateTime timestamp, StackTrace stackTrace, string str, Logging.Severity severity)
        {
            this.timestamp = timestamp;
            this.stackTrace = stackTrace;
            this.str = str;
            this.severity = severity;
        }

        public override string ToString()
        {
            return $"[{severity} | {timestamp.ToLongTimeString()} | {stackTrace.GetFrame(1)?.GetMethod()?.ReflectedType?.Name}] {str}";
        }
    }
}
