using Nukit.Console;

namespace Nukit
{
    internal static class ConsoleExtensions
    {
        public static IEnumerable<string> GetBanner()
        {
            var vsn = Program.GetVersion();

            string[] lines = [
                Program.GetProductName()?.Green() + (vsn != null ? $" version {vsn}".Yellow() : ""),
                Program.GetDescription()?.Italic() ?? "",
                Program.GetProjectUrl()?.Italic() ?? "",
                "Thank you for using my software.".Grey().Italic(),
                ""];

            return lines;
        }

        public static async Task<string[]> GetUpgradeNotice(this Tk.Nuget.INugetClient nuget)
        {
            var version = Program.GetVersion() ?? "";
            var result = await nuget.GetUpgradeVersionAsync("Nukit", version, false);

            if (result != null)
            {
                return [$"An upgrade is available".Yellow() + $" - check https://www.nuget.org/packages/{Program.GetProductName()}".Italic(), ""];
            }
            return [];
        }
    }
}
