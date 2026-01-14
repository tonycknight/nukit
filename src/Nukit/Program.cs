using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Tk.Nuget;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Nukit.Tests.Unit")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Nukit.Tests.Integration")]

namespace Nukit
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                var reg = CreateTypeRegister();
                var app = new CommandApp<Clearance.ClearanceCommand>(reg);

                app.Configure(c =>
                {
                    c.PropagateExceptions()
                     .ValidateExamples()
                     .UseAssemblyInformationalVersion()
                     .TrimTrailingPeriods(false);
                });

                return await app.RunAsync(args);
            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine($"Error: {ex.Message}");
                return 2;
            }
        }

        private static ITypeRegistrar CreateTypeRegister() => new TypeRegistrar(CreateServices());

        private static IServiceCollection CreateServices()
        {
            var result = new ServiceCollection();

            result.AddNugetClient()
                .AddTransient<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>()
                .AddTransient<FileSystem.IFileFinder, FileSystem.FileFinder>()
                .AddTransient<FileSystem.IFilePurger, FileSystem.FilePurger>()
                .AddTransient<Console.IConsoleWriter, Console.ConsoleWriter>();

            return result;
        }
    }
}
