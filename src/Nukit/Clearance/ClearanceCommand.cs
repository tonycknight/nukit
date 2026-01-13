using Nukit.Console;
using Spectre.Console.Cli;

namespace Nukit.Clearance
{
    internal class ClearanceCommand(IConsoleWriter console) : BaseCommand<ClearanceSettings>
    {
        protected override Task<bool> ExecuteCommandAsync(CommandContext context, ClearanceSettings settings, CancellationToken cancellationToken)
        {
            // TODO: if not force, then check IF NOT INTERACTIVE
            
            console.WriteLine($"Clearing path: {settings.Path}, Force: {settings.Force}".Cyan());

            return true.ToTaskResult();
        }
    }
}
