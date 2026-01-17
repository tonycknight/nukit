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
            if (!ConfirmPurge(settings))
                return false.ToTaskResult();

            var purgeResult = new FileSystem.FilePurgeInfo();
            var root = (settings.Path == "" ? "." : settings.Path).ResolveWorkingPath();
            
            if (settings.NukeBinaryDirectories)
            {
                purgeResult = PurgeBinaries(settings.DryRun, root).Add(purgeResult);
            }

            if (settings.NukeObjectDirectories)
            {
                purgeResult = PurgeObjects(settings.DryRun, root).Add(purgeResult);
            }

            foreach (var dirPattern in settings.NukeGlobbedDirectories.Coalesce())
            {
                purgeResult = PurgeDirectories(settings.DryRun, root, dirPattern).Add(purgeResult);
            }

            console.WriteSummary(purgeResult);

            return (purgeResult.Errors.Count == 0).ToTaskResult();
        }

        private FilePurgeInfo PurgeBinaries(bool dryRun, string root)
        {
            console.WriteDirectoryHeadline(root, "bin");
            
            var binDirs = fileFinder.FindBinaryDirectories(root);

            return PurgeDirectories(dryRun, binDirs);
        }

        private FilePurgeInfo PurgeObjects(bool dryRun, string root)
        {
            console.WriteDirectoryHeadline(root, "obj");

            var binDirs = fileFinder.FindObjectDirectories(root);

            return PurgeDirectories(dryRun, binDirs);
        }

        private FilePurgeInfo PurgeDirectories(bool dryRun, string root, string pattern)
        {
            console.WriteDirectoryHeadline(root, pattern);

            var dirs = fileFinder.FindGlobbedDirectories(root, pattern);

            return PurgeDirectories(dryRun, dirs);
        }

        private FilePurgeInfo PurgeDirectories(bool dryRun, IEnumerable<string> directories)
        {
            var purgeResult = new FileSystem.FilePurgeInfo();

            foreach (var directory in directories)
            {
                console.Write($"Deleting directory {directory.Cyan()}...".Indent(2));

                var result = purger.Delete(directory, dryRun);
                                
                console.WriteLineReport(result);
                var lines = result.Errors.Select(e => e.Red().Indent(2));
                console.WriteLines(lines);

                purgeResult = purgeResult.Add(result);
            }

            if (purgeResult.Found == 0)
                console.WriteLine("Nothing found.".Indent(2));

            return purgeResult;
        }

        private bool ConfirmPurge(ClearanceSettings settings)
        {
            // TODO: if not force, then check IF NOT INTERACTIVE
            if (settings.Force || settings.DryRun) return true;

            return console.Confirm("Confirm deletion?");
        }
    }
}