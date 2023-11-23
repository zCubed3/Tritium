using System;
using System.IO;

namespace Tritium.Logging
{
    // TODO: Timestamps?
    public class StreamLogWriter : ILogWriter
    {
        public readonly Stream targetStream;
        public readonly StreamWriter writer;

        public StreamLogWriter(Stream targetStream)
        {
            this.targetStream = targetStream;
            writer = new StreamWriter(targetStream);
        }

        public void Log(string message, LoggerInstance.LogSeverity severity)
        {
            // For the logfile we also log additional info on top
            // This is purely to track the order in which things occurred
            message = $"[{DateTime.UtcNow}] {message}";

            writer.WriteLine(message);
        }
    }
}
