using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Nukit.Clearance
{
    internal class ClearanceSettings : CommandSettings
    {

        [CommandArgument(0, "[path]")]
        [Description("The path to clear. Optional.")]
        public string Path { get; init; } = "";

        [CommandOption("-f|--force")]
        [Description("Force clearance without prompting.")]
        [DefaultValue(false)]
        public bool Force { get; init; } = false;

        [Description("Runs a scan, without any effect.")]
        [DefaultValue(false)]
        [CommandOption("--dry-run")]
        public bool DryRun { get; init; } = false;

        public override ValidationResult Validate()
        {
            return base.Validate();
        }
    }
}
