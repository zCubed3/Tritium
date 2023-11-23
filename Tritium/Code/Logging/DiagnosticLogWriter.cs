using System.Diagnostics;

namespace Tritium.Logging
{
    public class DiagnosticLogWriter : ILogWriter
    {
        public void Log(string message, LoggerInstance.LogSeverity severity) => Debug.WriteLine(message);
    }
}
