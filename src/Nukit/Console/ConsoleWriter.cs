using Spectre.Console;

namespace Nukit.Console
{
    internal record Message
    {
        public required bool IsVerbose { get; init; }
        public required IReadOnlyList<string> Text { get; init; }
    }

    internal static class MessageExtensions
    {
        public static Message ToMessage(this IEnumerable<string> lines, bool verbose = true) =>
            new Message() { IsVerbose = verbose, Text = [.. lines] };

        public static Message ToMessage(this string line, bool verbose = true) =>
            new Message() { IsVerbose = verbose, Text = [line] };
    }

    internal interface IConsoleWriter
    {
        void WriteLine(Message message);
        void Write(Message message);
        bool Confirm(string message);
    }

    internal class ConsoleWriter : IConsoleWriter
    {
        private readonly IAnsiConsole _ansiConsole = AnsiConsole.Console;

        public bool Confirm(string message) => _ansiConsole.Confirm(message, false);

        public void Write(Message message)
        {
            foreach (var line in message.Text)
            {
                _ansiConsole.Markup(line);
            }
        }

        public void WriteLine(Message message)
        {
            foreach (var line in message.Text)
            {
                _ansiConsole.MarkupLine(line);
            }
        }
    }
}
