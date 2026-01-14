using System.ComponentModel;
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

        [Description("Nuke binary directories.")]
        [DefaultValue(true)]
        [CommandOption("--bin")]
        public bool NukeBinaryDirectories { get; init; } = true;

        [Description("Nuke object directories.")]
        [DefaultValue(true)]
        [CommandOption("--obj")]
        public bool NukeObjectDirectories { get; init; } = true;

        [Description("Nuke test results.")]
        [DefaultValue(false)]
        [CommandOption("--trx")]
        public bool NukeTestResultDirectories { get; init; } = false;

        [Description("Nuke directories matching a glob pattern. Multiple patterns may be given.")]
        [CommandOption("--glob", IsHidden = true)]
        public string[] NukeGlobbedDirectories { get; init; } = [];
    }
}
