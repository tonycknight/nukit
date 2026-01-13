using System.ComponentModel;
using Spectre.Console.Cli;

namespace Nukit.Clearance
{
    internal class ClearanceSettings : CommandSettings
    {
        [CommandOption("-f|--force")]
        [Description("Force clearance without prompting.")]
        [DefaultValue(false)]
        public bool Force { get; init; } = false;
    }
}
