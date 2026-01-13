namespace Nukit
{
    internal static class Extensions
    {
        public static Task<T> ToTaskResult<T>(this T value) => Task.FromResult(value);
    }
}
