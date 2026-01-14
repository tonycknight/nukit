namespace Nukit
{
    internal static class Extensions
    {
        public static Task<T> ToTaskResult<T>(this T value) => Task.FromResult(value);

        public static IEnumerable<T> Coalesce<T>(this IEnumerable<T>? source) =>
            source ?? Enumerable.Empty<T>();

        public static string Indent(this string value, int chars) => $"{new string(' ', chars)}{value}";
    }
}
