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
                                   
            var result = PurgeBinaries(settings.DryRun, root);
            purgeResult = purgeResult.Add(result);

            result = PurgeObjects(settings.DryRun, root);
            purgeResult = purgeResult.Add(result);
                        
            result = PurgeTestResults(settings.DryRun, root);
            purgeResult = purgeResult.Add(result);

            WriteSummary(purgeResult);

            return (purgeResult.Errors.Count == 0).ToTaskResult();
        }
                
        private FilePurgeInfo PurgeBinaries(bool dryRun, string root)
        {
            console.WriteLine($"Searching for 'bin' directories under {root}".Yellow());

            var binDirs = fileFinder.FindBinaryDirectories(root);

            return PurgeDirectories(dryRun, binDirs);
        }

        private FilePurgeInfo PurgeObjects(bool dryRun, string root)
        {
            console.WriteLine($"Searching for 'obj' directories under {root}".Yellow());

            var binDirs = fileFinder.FindObjectDirectories(root);

            return PurgeDirectories(dryRun, binDirs);
        }

        private FilePurgeInfo PurgeTestResults(bool dryRun, string root)
        {
            console.WriteLine($"Searching for 'TestResults' directories under {root}".Yellow());

            var binDirs = fileFinder.FindTestResultDirectories(root);

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
                // TODO: any errors... enumerate
                foreach (var error in result.Errors)
                {
                    console.WriteLine(error.Red());
                }

                purgeResult = purgeResult.Add(result);
            }

            if (purgeResult.Found == 0)
                console.WriteLine("None found.");

            return purgeResult;
        }

        private void WriteSummary(FilePurgeInfo result)
        {
            console.WriteLine("Nuke summary:".Yellow());            
            var msg = result switch
            {
                { Errors.Count: > 0 } => $"Found: {result.Found.ToString().Cyan()} Deleted: {result.Deleted.ToString().Green()} Erros: {result.Errors.Count.ToString().Red()}",
                { Deleted: 0 } => $"Found: {result.Found.ToString().Cyan()} Deleted: {result.Deleted.ToString().Yellow()} Erros: {result.Errors.Count.ToString().Yellow()}",
                _ => $"Found: {result.Found.ToString().Cyan()} Deleted: {result.Deleted.ToString().Green()} Erros: {result.Errors.Count.ToString().Yellow()}",
            };

            console.WriteLine("  " + msg);
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
                { Errors.Count: > 0 } => "done.".Red(),
                { Deleted: > 0} => " done.".Green(),
                _ => " done."
            };

            var baseMsg = $" {result.Found.ToString().Cyan()}/{result.Deleted.ToString().Green()}/{result.Errors.Count.ToString().Red()}";

            return msg + baseMsg;
        }
    }
}
