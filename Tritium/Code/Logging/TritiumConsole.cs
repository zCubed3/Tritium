using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace Tritium.Logging
{
    public class TritiumConsole : ILogWriter
    {
        public static readonly TritiumConsole Instance = new TritiumConsole();

        public class ConsoleEntry
        {
            public ConsoleEntry() { }
            public ConsoleEntry(string message, Color color)
            {
                this.message = message;
                this.color = color;
            }

            public string message;
            public Color color = Color.White;
        }

        public readonly Queue<ConsoleEntry> entries = new Queue<ConsoleEntry>();

        protected int maxEntries = 1000;

        public void AddEntry(string message, Color color)
        {
            while (entries.Count > maxEntries)
                entries.Dequeue();

            entries.Enqueue(new ConsoleEntry(message, color));
        }

        public void AddEntry(string message) => AddEntry(message, Color.White);
        
        public void Log(string message, LoggerInstance.LogSeverity severity)
        {
            Color logColor = Color.White;
            switch (severity)
            {
                default:
                    break;

                case LoggerInstance.LogSeverity.Warning:
                    logColor = Color.Yellow;
                    break;

                case LoggerInstance.LogSeverity.Error:
                    logColor = Color.Red;
                    break;
            }

            AddEntry(message, logColor);
        }
    }
}
