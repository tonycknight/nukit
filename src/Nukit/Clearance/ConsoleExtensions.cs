using System.IO;
using System.Runtime.CompilerServices;
using Nukit.Console;
using Nukit.FileSystem;

namespace Nukit.Clearance
{
    internal static class ConsoleExtensions
    {
        public static void WriteDirectoryHeadline(this IConsoleWriter console, string path, string name)
        {
            var line = $"Searching for {name.Cyan()} directories under {path.CornflowerBlue()}";

            console.WriteLine(line);
        }

        public static void WriteDirectoryDeletionPrefix(this IConsoleWriter console, string directory)
        {
            console.Write($"Deleting directory {directory.Cyan()}...".Indent(2));
        }

        public static void WriteLineReport(this IConsoleWriter console, FilePurgeInfo result)
        {
            console.WriteLine(GetLineReport(result));
            var lines = result.Errors.Select(e => e.Red().Indent(2));
            console.WriteLines(lines);
        }

        public static void WriteEmptyResult(this IConsoleWriter console) => console.WriteLine("Nothing found.".Indent(2));

        public static void WriteSummary(this IConsoleWriter console, FilePurgeInfo result) => console.WriteLine(GetSummary(result));

        private static string GetLineReport(FilePurgeInfo result)
        {
            var msg = result switch
            {
                { Errors.Count: > 0 } => "done.".Red(),
                { Deleted: > 0 } => " done.".Green(),
                _ => " done.".Cyan()
            };

            var found = result.Found.ToString().Cyan();
            var deleted = result.Deleted > 0 ? result.Deleted.ToString().Green() : result.Deleted.ToString().Yellow();
            var errors = result.Errors.Count > 0 ? result.Errors.Count.ToString().Red() : result.Errors.Count.ToString().Yellow();

            var baseMsg = $" {found}/{deleted}/{errors}";

            return msg + baseMsg;
        }

        private static string GetSummary(FilePurgeInfo result)
        {
            var found = result switch
            {
                { Found: 0 } => $"Found: {result.Found.ToString().Yellow()}",
                _ => $"Found: {result.Found.ToString().Cyan()}"
            };

            var deleted = result switch
            {
                { Deleted: 0 } => $"Deleted: {result.Deleted.ToString().Yellow()}",
                _ => $"Deleted: {result.Deleted.ToString().Green()}",
            };
            var errors = result switch
            {
                { Errors.Count: > 0 } => $"Errors: {result.Errors.Count.ToString().Red()}",
                _ => $"Errors: {result.Errors.Count.ToString().Yellow()}",
            };

            return "Nuke summary: ".Yellow() + $"{found} {deleted} {errors}";
        }
    }
}
