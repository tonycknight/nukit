namespace Nukit.Console
{
    internal static class MarkupExtensions
    {
        public static string Grey(this string text) => $"[grey]{text}[/]";
        public static string Green(this string text) => $"[green]{text}[/]";
        public static string Red(this string text) => $"[red]{text}[/]";
        public static string Cyan(this string text) => $"[cyan]{text}[/]";
        public static string Yellow(this string text) => $"[yellow]{text}[/]";
    }
}
