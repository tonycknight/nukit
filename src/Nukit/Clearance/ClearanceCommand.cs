using System.Reflection;
using NuGet.Configuration;
using Nukit.Console;
using Nukit.FileSystem;
using Spectre.Console.Cli;

namespace Nukit.Clearance
{
    internal class ClearanceCommand(IConsoleWriter console, IFileFinder fileFinder, IFilePurger purger) : BaseCommand<ClearanceSettings>
    {
        protected override Task<bool> ExecuteCommandAsync(CommandContext context, ClearanceSettings settings, CancellationToken cancellationToken)
        {
            if (!ConfirmPurge(settings))
                return false.ToTaskResult();
            
            var purgeResult = new FileSystem.FilePurgeInfo();
            var root = settings.Path == "" ? "." : settings.Path; // TODO: reconcile to current directory
                       
            console.WriteLine($"Searching for 'bin' directories under {root}".Yellow());
            var result = PurgeBinaries(settings.DryRun, root);
            purgeResult = purgeResult with
            {
                Found = purgeResult.Found + result.Found,
                Deleted = purgeResult.Deleted + result.Deleted,
                Errors = purgeResult.Errors + result.Errors
            };

            // TODO: obj directories

            console.WriteLine("Purge Summary:".Yellow());
            console.WriteLine($"  Found:   {purgeResult.Found}".Cyan());
            console.WriteLine($"  Deleted: {purgeResult.Deleted}".Green());
            console.WriteLine($"  Errors:  {purgeResult.Errors}".Red());

            return true.ToTaskResult();
        }

        private FilePurgeInfo PurgeBinaries(bool dryRun, string root)
        {
            var binDirs = fileFinder.FindBinaryDirectories(root);

            return PurgeDirectories(dryRun, binDirs);
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

                purgeResult = purgeResult with
                {
                    Found = purgeResult.Found + result.Found,
                    Deleted = purgeResult.Deleted + result.Deleted,
                    Errors = purgeResult.Errors + result.Errors
                };
            }

            return purgeResult;
        }

        private bool ConfirmPurge(ClearanceSettings settings)
        {
            // TODO: if not force, then check IF NOT INTERACTIVE
            if (settings.Force) return true;

            return console.Confirm("Confirm deletion?");
        }

        private string GetLineReport(FilePurgeInfo result)
        {
            var msg = result switch
            {
                { Errors: > 0 } => "done.".Red(),
                { Deleted: > 0} => " done.".Green(),
                _ => " done."
            };

            var baseMsg = $" {result.Found.ToString().Cyan()}/{result.Deleted.ToString().Green()}/{result.Errors.ToString().Red()}";

            return msg + baseMsg;
        }
    }
}
