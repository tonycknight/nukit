using Spectre.Console.Cli;

namespace Nukit
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                var app = new CommandApp(); // TODO: inject DI container

                app.Configure(c =>
                {
                    c.PropagateExceptions()
                     .ValidateExamples()
                     .UseAssemblyInformationalVersion()
                     .TrimTrailingPeriods(false);

                    c.AddCommand<Clearance.ClearanceCommand>("clear").WithDescription("Clear out project folder build artifacts.");
                });

                return await app.RunAsync(args);
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"Error: {ex.Message}");
                return 2;
            }
        }
    }
}
