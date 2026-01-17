using System.ComponentModel;
using Spectre.Console.Cli;

namespace Nukit
{
    internal abstract class BaseCommandSettings : CommandSettings
    {
        [Description("Show or hide the application banner.")]
        [DefaultValue(false)]
        [CommandOption("--no-banner <true|false>")]
        public bool NoBanner { get; init; } = false;
    }
}
