using Spectre.Console;

namespace Nukit.Console
{
    internal class ConsoleWriter
    {
        private readonly IAnsiConsole _ansiConsole = AnsiConsole.Console;

        public void WriteLine(string message) => _ansiConsole.WriteLine(message);
    }
}
