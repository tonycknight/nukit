using System.Reflection;
using Nukit.Console;
using Spectre.Console.Cli;

namespace Nukit
{
    internal abstract class BaseCommand<T>(IConsoleWriter console, Tk.Nuget.INugetClient nuget) : AsyncCommand<T>
        where T : CommandSettings
    {
        public override async Task<int> ExecuteAsync(CommandContext context, T settings, CancellationToken cancellationToken)
        {
            await ShowBannerAsync();

            var result = await ExecuteCommandAsync(context, settings, cancellationToken);

            return result ? 0 : 1;
        }

        private async Task ShowBannerAsync()
        {
            var vsn = GetVersion();
            console.WriteLine("Nukit".Green() + (vsn != null ? $" version {vsn}".Yellow() : ""));
            console.WriteLine("https://github.com/tonycknight/nukit".Italic());

            console.WriteLine("Thank you for using my software.".Grey().Italic());
            console.WriteLine("");

            await CheckForUpgradesAsync();
        }

        private async Task CheckForUpgradesAsync()
        {
            var version = GetVersion() ?? "";
            var result = await nuget.GetUpgradeVersionAsync("Nukit", version, false);

            if (result != null)
            {
                console.WriteLine($"An upgrade is available".Yellow().Italic());
                console.WriteLine("");
            }
        }

        private string? GetVersion() => Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;

        protected abstract Task<bool> ExecuteCommandAsync(CommandContext context, T settings, CancellationToken cancellationToken);
    }
}
