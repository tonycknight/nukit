using Nukit.Console;
using Nukit.FileSystem;
using Spectre.Console.Cli;

namespace Nukit.Clearance
{
    internal class ClearanceCommand(IConsoleWriter console, Tk.Nuget.INugetClient nuget, IFileFinder fileFinder, IFilePurger purger)
#pragma warning disable CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
        : BaseCommand<ClearanceSettings>(console, nuget)
#pragma warning restore CS9107 // Parameter is captured into the state of the enclosing type and its value is also passed to the base constructor. The value might be captured by the base class as well.
    {
        protected override Task<bool> ExecuteCommandAsync(CommandContext context, ClearanceSettings settings, CancellationToken cancellationToken)
        {
            if (!ConfirmPurge(settings))
                return false.ToTaskResult();

            var purgeResult = new FileSystem.FilePurgeInfo();
            var root = fileFinder.Normalise(settings.Path == "" ? "." : settings.Path);

            if (settings.NukeBinaryDirectories)
            {
                var result = PurgeBinaries(settings.DryRun, root);
                purgeResult = purgeResult.Add(result);
            }

            if (settings.NukeObjectDirectories)
            {
                var result = PurgeObjects(settings.DryRun, root);
                purgeResult = purgeResult.Add(result);
            }

            if (settings.NukeTestResultDirectories)
            {
                var result = PurgeTestResults(settings.DryRun, root);
                purgeResult = purgeResult.Add(result);
            }

            foreach (var dirPattern in settings.NukeGlobbedDirectories.Coalesce())
            {
                var result = PurgeDirectories(settings.DryRun, root, dirPattern);
                purgeResult = purgeResult.Add(result);
            }

            WriteSummary(purgeResult);

            return (purgeResult.Errors.Count == 0).ToTaskResult();
        }

        private FilePurgeInfo PurgeBinaries(bool dryRun, string root)
        {
            console.WriteLine(GetDirectoryHeadline(root, "bin"));

            var binDirs = fileFinder.FindBinaryDirectories(root);

            return PurgeDirectories(dryRun, binDirs);
        }

        private FilePurgeInfo PurgeObjects(bool dryRun, string root)
        {
            console.WriteLine(GetDirectoryHeadline(root, "obj"));

            var binDirs = fileFinder.FindObjectDirectories(root);

            return PurgeDirectories(dryRun, binDirs);
        }

        private FilePurgeInfo PurgeTestResults(bool dryRun, string root)
        {
            console.WriteLine(GetDirectoryHeadline(root, "TestResults"));

            var binDirs = fileFinder.FindTestResultDirectories(root);

            return PurgeDirectories(dryRun, binDirs);
        }

        private FilePurgeInfo PurgeDirectories(bool dryRun, string root, string pattern)
        {
            console.WriteLine(GetDirectoryHeadline(root, pattern));

            var dirs = fileFinder.FindDirectories(root, pattern);

            return PurgeDirectories(dryRun, dirs);
        }

        private FilePurgeInfo PurgeDirectories(bool dryRun, IEnumerable<DirectoryInfo> directories)
        {
            var purgeResult = new FileSystem.FilePurgeInfo();

            foreach (var directory in directories)
            {
                console.Write($"Deleting directory {directory.FullName.Cyan()}...");

                var result = purger.Delete(directory, dryRun);

                var report = GetLineReport(result);

                console.WriteLine(report);

                foreach (var error in result.Errors)
                {
                    console.WriteLine(error.Red());
                }

                purgeResult = purgeResult.Add(result);
            }

            if (purgeResult.Found == 0)
                console.WriteLine("Nothing found.");

            return purgeResult;
        }

        private void WriteSummary(FilePurgeInfo result)
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
                { Errors.Count: > 0 } => $"Erros: {result.Errors.Count.ToString().Red()}",
                _ => $"Erros: {result.Errors.Count.ToString().Yellow()}",
            };

            console.WriteLine("Nuke summary: ".Yellow() + $"{found} {deleted} {errors}");
        }


        private bool ConfirmPurge(ClearanceSettings settings)
        {
            // TODO: if not force, then check IF NOT INTERACTIVE
            if (settings.Force || settings.DryRun) return true;

            return console.Confirm("Confirm deletion?");
        }

        private string GetLineReport(FilePurgeInfo result)
        {
            var msg = result switch
            {
                { Errors.Count: > 0 } => "done.".Red(),
                { Deleted: > 0 } => " done.".Green(),
                _ => " done."
            };

            var found = result.Found.ToString().Cyan();
            var deleted = result.Deleted.ToString().Green();
            var errors = result.Errors.Count > 0 ? result.Errors.Count.ToString().Red() : result.Errors.Count.ToString().Yellow();

            var baseMsg = $" {found}/{deleted}/{errors}";

            return msg + baseMsg;
        }

        private string GetDirectoryHeadline(string path, string name) => $"Searching for '{name.Cyan()}' directories under {path.CornflowerBlue()}";
    }
}