using Spectre.Console.Cli;

namespace Nukit
{
    internal abstract class BaseCommand<T> : AsyncCommand<T>
        where T : CommandSettings
    {
        public override async Task<int> ExecuteAsync(CommandContext context, T settings, CancellationToken cancellationToken)
        {
            await CheckForUpgrades();

            var result = await ExecuteCommandAsync(context, settings, cancellationToken);

            return result ? 0 : 1;
        }

        private Task CheckForUpgrades()
        {
            // TODO: implement upgrade check logic once package is first published
            return Task.CompletedTask;
        }

        protected abstract Task<bool> ExecuteCommandAsync(CommandContext context, T settings, CancellationToken cancellationToken);
    }
}
