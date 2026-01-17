using Spectre.Console;

namespace Nukit.Console
{
    internal interface IConsoleWriter
    {
        void WriteLines(IEnumerable<string> messages);
        void WriteLine(string message);
        void Write(string message);
        bool Confirm(string message);
    }

    internal class ConsoleWriter : IConsoleWriter
    {
        private readonly IAnsiConsole _ansiConsole = AnsiConsole.Console;

        public bool Confirm(string message) => _ansiConsole.Confirm(message, false);

        public void Write(string message) => _ansiConsole.Markup(message);

        public void WriteLine(string message) => _ansiConsole.MarkupLine(message);

        void IConsoleWriter.WriteLines(IEnumerable<string> messages)
        {
            foreach (var msg in messages)
            {
                WriteLine(msg);
            }
        }
    }
}
