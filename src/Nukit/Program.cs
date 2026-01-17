using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Nukit.Console;
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
                var app = new CommandApp<Clearance.ClearanceCommand>(reg).WithDescription($"{Program.GetDescription()} {Program.GetProjectUrl()?.Italic() ?? String.Empty}");

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

        private static IServiceCollection CreateServices() =>
            new ServiceCollection()
                .AddNugetClient()
                .AddTransient<FileSystem.IDirectoryProvider, FileSystem.DirectoryProvider>()
                .AddTransient<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>()
                .AddTransient<FileSystem.IDirectoryFinder, FileSystem.DirectoryFinder>()
                .AddTransient<FileSystem.IDirectoryPurger, FileSystem.DirectoryPurger>()
                .AddTransient<Console.IConsoleWriter, Console.ConsoleWriter>();

        public static string? GetVersion() => Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

        public static string? GetProductName() => Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyProductAttribute>()?.Product;

        public static string GetDescription() => Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyDescriptionAttribute>()?.Description ?? "";

        public static string? GetProjectUrl() => Assembly.GetExecutingAssembly()
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .Where(t => t.Key == "PackageProjectUrl" || t.Key == "RepositoryUrl")
                .Select(t => t.Value)
                .FirstOrDefault();
    }
}
