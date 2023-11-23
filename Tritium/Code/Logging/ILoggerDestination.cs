namespace Tritium.Logging
{
    public interface ILogWriter
    {
        public void Log(string message, LoggerInstance.LogSeverity severity);
    }
}
