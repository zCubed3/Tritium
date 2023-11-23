using System;

namespace Tritium.Logging
{
    public class ConsoleLogWriter : ILogWriter
    {
        public void Log(string message, LoggerInstance.LogSeverity severity)
        {
            switch (severity)
            {
                default:
                    break;

                case LoggerInstance.LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LoggerInstance.LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
