using Nukit.Console;
using Spectre.Console.Cli;

namespace Nukit
{
    internal abstract class BaseCommand<T>(IConsoleWriter console, Tk.Nuget.INugetClient nuget) : AsyncCommand<T>
        where T : BaseCommandSettings
    {
        public override async Task<int> ExecuteAsync(CommandContext context, T settings, CancellationToken cancellationToken)
        {
            if (!settings.NoBanner)
                await ShowBannerAsync();

            var result = await ExecuteCommandAsync(context, settings, cancellationToken);

            return result ? 0 : 1;
        }

        private async Task ShowBannerAsync()
        {
            var lines = ConsoleExtensions.GetBanner();
            var upgrades = await nuget.GetUpgradeNotice();

            console.WriteLines(lines);
            console.WriteLines(upgrades);
        }

        protected abstract Task<bool> ExecuteCommandAsync(CommandContext context, T settings, CancellationToken cancellationToken);
    }
}
