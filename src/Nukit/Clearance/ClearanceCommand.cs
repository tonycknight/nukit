using Nukit.Console;
using Nukit.FileSystem;
using Spectre.Console.Cli;
using Tk.Extensions.Io;

namespace Nukit.Clearance
{
    internal class ClearanceCommand(IConsoleWriter console, Tk.Nuget.INugetClient nuget, IDirectoryFinder fileFinder, IDirectoryPurger purger)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
        : BaseCommand<ClearanceSettings>(console, nuget)
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    {
        protected override Task<bool> ExecuteCommandAsync(CommandContext context, ClearanceSettings settings, CancellationToken cancellationToken)
        {
            if (!console.ConfirmPurge(settings))
                return false.ToTaskResult();

            var purgeResult = new FileSystem.FilePurgeInfo();
            var root = (settings.Path == "" ? "." : settings.Path).ResolveWorkingPath();

            if (settings.NukeBinaryDirectories)
            {
                purgeResult = PurgeBinaries(settings, root).Add(purgeResult);
            }

            if (settings.NukeObjectDirectories)
            {
                purgeResult = PurgeObjects(settings, root).Add(purgeResult);
            }

            foreach (var dirPattern in settings.NukeGlobbedDirectories.Coalesce())
            {
                purgeResult = PurgeDirectories(settings, root, dirPattern).Add(purgeResult);
            }

            console.WriteSummary(purgeResult);

            return (purgeResult.Errors.Count == 0).ToTaskResult();
        }

        private FilePurgeInfo PurgeBinaries(ClearanceSettings settings, string root)
        {
            console.WriteDirectoryHeadline(root, "bin");

            var binDirs = fileFinder.FindBinaryDirectories(root);

            return PurgeDirectories(settings, binDirs);
        }

        private FilePurgeInfo PurgeObjects(ClearanceSettings settings, string root)
        {
            console.WriteDirectoryHeadline(root, "obj");

            var binDirs = fileFinder.FindObjectDirectories(root);

            return PurgeDirectories(settings, binDirs);
        }

        private FilePurgeInfo PurgeDirectories(ClearanceSettings settings, string root, string pattern)
        {
            console.WriteDirectoryHeadline(root, pattern);

            var dirs = fileFinder.FindGlobbedDirectories(root, pattern);

            return PurgeDirectories(settings, dirs);
        }

        private FilePurgeInfo PurgeDirectories(ClearanceSettings settings, IEnumerable<string> directories)
        {
            var purgeResult = new FileSystem.FilePurgeInfo();

            foreach (var directory in directories)
            {
                console.WriteDirectoryDeletionPrefix(directory);

                var result = purger.Delete(directory, settings.DryRun, settings.Retries);

                console.WriteLineReport(result);

                purgeResult = purgeResult.Add(result);
            }

            if (purgeResult.Found == 0)
                console.WriteEmptyResult();

            return purgeResult;
        }
    }
}