namespace Nukit.Console
{
    internal static class MarkupExtensions
    {
        public static string Cyan(this string text) => $"[cyan]{text}[/]";
        public static string Yellow(this string text) => $"[yellow]{text}[/]";
    }
}
