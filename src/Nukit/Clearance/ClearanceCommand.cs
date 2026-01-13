using Spectre.Console.Cli;

namespace Nukit.Clearance
{
    internal class ClearanceCommand : BaseCommand<ClearanceSettings>
    {
        protected override Task<bool> ExecuteCommandAsync(CommandContext context, ClearanceSettings settings, CancellationToken cancellationToken)
        {
            // TODO: if not force, then check IF NOT INTERACTIVE
            // 
            return true.ToTaskResult();
        }
    }
}
