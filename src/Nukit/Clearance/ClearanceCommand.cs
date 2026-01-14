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
            var root = fileFinder.Normalise(settings.Path == "" ? "." : settings.Path);
                                   
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
            console.WriteLine("Searching for 'bin' directories under ".Cyan() + root.CornflowerBlue());

            var binDirs = fileFinder.FindBinaryDirectories(root);

            return PurgeDirectories(dryRun, binDirs);
        }

        private FilePurgeInfo PurgeObjects(bool dryRun, string root)
        {
            console.WriteLine("Searching for 'obj' directories under ".Cyan() + root.CornflowerBlue());

            var binDirs = fileFinder.FindObjectDirectories(root);

            return PurgeDirectories(dryRun, binDirs);
        }

        private FilePurgeInfo PurgeTestResults(bool dryRun, string root)
        {
            console.WriteLine("Searching for 'TestResults' directories under ".Cyan() + root.CornflowerBlue());

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
                { Deleted: > 0} => " done.".Green(),
                _ => " done."
            };

            var found = result.Found.ToString().Cyan();
            var deleted = result.Deleted.ToString().Green();
            var errors = result.Errors.Count > 0 ? result.Errors.Count.ToString().Red() : result.Errors.Count.ToString().Yellow();

            var baseMsg = $" {found}/{deleted}/{errors}";

            return msg + baseMsg;
        }
    }
}
