using Spectre.Console;

namespace Nukit.Console
{
    internal interface IConsoleWriter
    {
        void WriteLine(Message message);
        void Write(Message message);
        bool Confirm(string message);
    }

    internal class ConsoleWriter(BaseCommandSettings settings) : IConsoleWriter
    {
        private readonly IAnsiConsole _ansiConsole = AnsiConsole.Console;

        public bool Confirm(string message) => _ansiConsole.Confirm(message, false);

        public void Write(Message message) => WriteMarkup(message, _ansiConsole.Markup);

        public void WriteLine(Message message) => WriteMarkup(message, _ansiConsole.MarkupLine);

        private void WriteMarkup(Message message, Action<string> write)
        {
            if (settings.VerboseLogging || message.IsVerbose)
            {
                foreach (var line in message.Text)
                {
                    write(line);
                }
            }
        }
    }
}
