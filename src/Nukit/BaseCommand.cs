using Nukit.Console;
using Spectre.Console.Cli;

namespace Nukit
{
    internal abstract class BaseCommand<T>(IConsoleWriterFactory consoleFactory, Tk.Nuget.INugetClient nuget) : AsyncCommand<T>
        where T : BaseCommandSettings
    {
        protected IConsoleWriter console;

        public override async Task<int> ExecuteAsync(CommandContext context, T settings, CancellationToken cancellationToken)
        {
            console = consoleFactory.CreateWriter(settings);

            if (!settings.NoBanner && settings.VerboseLogging)
                await ShowBannerAsync();

            var result = await ExecuteCommandAsync(context, settings, cancellationToken);

            return result ? 0 : 1;
        }

        private async Task ShowBannerAsync()
        {
            var lines = ConsoleExtensions.GetBanner();
            var upgrades = await nuget.GetUpgradeNotice();

            console.WriteLine(lines.ToMessage());
            console.WriteLine(upgrades.ToMessage());
        }

        protected abstract Task<bool> ExecuteCommandAsync(CommandContext context, T settings, CancellationToken cancellationToken);
    }
}
