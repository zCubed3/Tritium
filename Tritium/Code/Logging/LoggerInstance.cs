using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Tritium.Logging
{
    public class LoggerInstance
    {
        public enum LogSeverity
        {
            None,
            Info,
            Warning,
            Error
        }

        public string prefix = "[LOGGER] ";
        
        protected List<ILogWriter> writers = new List<ILogWriter>() { 
            new ConsoleLogWriter(), 
            new DiagnosticLogWriter()
        };

#if DEBUG
        public static LoggerInstance DebugLogger = new LoggerInstance();
#endif

        public LoggerInstance() { }
        public LoggerInstance(string prefix)
        {
            this.prefix = prefix;
        }

        public virtual void AddWriter(ILogWriter writer) => writers.Add(writer);

        protected virtual string GetPrefix(LogSeverity severity)
        {
            string severityStr = "";

            if (severity > LogSeverity.Info)
                severityStr = $"[{severityStr}]";
                    
            return $"[{prefix}] {severityStr}";
        }
        
        protected virtual string FormatMessage(string message, LogSeverity severity)
        {
            return $"{GetPrefix(severity)}{message}";
        }

        public virtual void Log(string message, LogSeverity severity = LogSeverity.None)
        {
            string msg = FormatMessage(message, severity);

            foreach (ILogWriter destination in writers)
                destination.Log(msg, severity);
        }
    }
}
