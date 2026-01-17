using Nukit.Console;

namespace Nukit.Clearance
{
    internal static class ConsoleExtensions
    {
        public static void WriteDirectoryHeadline(this IConsoleWriter console, string path, string name)
        {
            var line = $"Searching for {name.Cyan()} directories under {path.CornflowerBlue()}";

            console.WriteLine(line);
        }
    }
}
