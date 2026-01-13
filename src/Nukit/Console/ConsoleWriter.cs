using Spectre.Console;

namespace Nukit.Console
{
    internal interface IConsoleWriter
    {
        void WriteLine(string message);
        void Write(string message);
    }

    internal class ConsoleWriter : IConsoleWriter
    {
        private readonly IAnsiConsole _ansiConsole = AnsiConsole.Console;

        public void Write(string message) => _ansiConsole.Markup(message);
        public void WriteLine(string message) => _ansiConsole.MarkupLine(message);
    }
}
