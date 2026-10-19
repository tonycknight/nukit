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
}
