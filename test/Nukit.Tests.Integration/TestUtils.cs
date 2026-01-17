using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Shouldly;
using Xunit.Abstractions;

namespace Nukit.Tests.Integration
{
    internal record ProcessExecution
    {
        internal string Command { get; init; } = "";
        internal int ExitCode { get; init; }
        internal string OutLog { get; init; } = "";
        internal string ErrorLog { get; init; } = "";
    }

    internal class OutputDirectory(string path) : IDisposable
    {
        public string Path => path;

        public void Dispose()
        {
            Directory.Delete(path, true);
        }
    }

    internal static class TestUtils
    {
        private const string httpPackage = "System.Net.Http";

        public static (string, string) CommandArgs(string cmd)
        {
            var idx = cmd.IndexOf(" ");
            if (idx > 0)
                return (cmd.Substring(0, idx), cmd.Substring(idx + 1));
            else
                return (cmd, "");
        }

        public static OutputDirectory GetOutDir() => new OutputDirectory($"./testdata/{Guid.NewGuid().ToString().Replace("-", "")}");

        public static string CreateProjectCommand(this OutputDirectory outDir) => $"dotnet new classlib -o ./{outDir.Path} -n testproj";
        public static string DotnetCleanCommand(this OutputDirectory outDir) => $"dotnet clean ./{outDir.Path}/testproj.csproj";

        public static string AddGoodHttpPackageACommand(this OutputDirectory outDir) => $"dotnet add ./{outDir.Path}/testproj.csproj package {httpPackage} -v 4.3.4";

        public static string DotnetBuildCommand(this OutputDirectory outDir) => $"dotnet build ./{outDir.Path}/testproj.csproj";

        public static string NukitCommand(this OutputDirectory outDir, bool dryRun, bool nukeBin = true, bool nukeObj = true) => $"dotnet nukit.dll {outDir.Path}/ --dry-run {dryRun} --bin {nukeBin} --obj {nukeObj} --trx --force";

        public static ProcessExecution[] Execute(this string[] cmds, ITestOutputHelper output, bool success) =>
            cmds.Select(c => c.Execute(output, true)).ToArray();

        public static ProcessExecution Execute(this string args, ITestOutputHelper output, bool success)
        {
            using var process = CreateProcess(args);

            var result = process
                .LogProcessArgs(output)
                .Execute()
                .LogProcessExecution(output);

            return success
                ? result.AssertSuccess()
                : result.AssertFailure();
        }

        public static Process CreateProcess(string command)
        {
            var (exec, args) = CommandArgs(command);

            var proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.FileName = exec;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            return proc;
        }

        public static ProcessExecution Execute(this Process process)
        {
            var ok = process.Start();

            if (!ok)
                throw new InvalidOperationException("Cannot start process");

            var outLog = process.StandardOutput.ReadToEnd();
            var errLog = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return new ProcessExecution() { Command = $"{process.StartInfo.FileName} {process.StartInfo.Arguments}", ExitCode = process.ExitCode, OutLog = outLog, ErrorLog = errLog };
        }

        public static Process LogProcessArgs(this Process process, ITestOutputHelper output)
        {
            var msg = $"Running command:{Environment.NewLine}{process.StartInfo.FileName} {process.StartInfo.Arguments}";

            output.WriteLine(msg);

            return process;
        }

        public static ProcessExecution LogProcessExecution(this ProcessExecution args, ITestOutputHelper output)
        {
            if (args.OutLog != "")
            {
                output.WriteLine($"Output:{Environment.NewLine}{args.OutLog}");
            }
            else
            {
                output.WriteLine($"Error:{Environment.NewLine}{args.ErrorLog}");
            }

            return args;
        }

        public static ProcessExecution AssertSuccess(this ProcessExecution args)
        {
            args.ExitCode.ShouldBe(0);
            args.OutLog.ShouldNotBeNullOrWhiteSpace();
            args.ErrorLog.ShouldBeEmpty();
            return args;
        }

        public static ProcessExecution AssertFailure(this ProcessExecution args)
        {
            args.ExitCode.ShouldNotBe(0);
            args.OutLog.ShouldBeEmpty();
            args.ErrorLog.ShouldNotBeNullOrWhiteSpace();
            return args;
        }

        public static ProcessExecution VerifyNukitSummary(this ProcessExecution result, int found, int deleted, int errors)
        {
            var reportLine = result.OutLog.Split(Environment.NewLine, StringSplitOptions.TrimEntries)
                .Single(s => s.StartsWith("Nuke summary: "));

            var pattern = @"Nuke summary: Found:\s*(\d+)\s+Deleted:\s*(\d+)\s+Errors:\s*(\d+)";
            var match = Regex.Match(reportLine, pattern);

            int.Parse(match.Groups[1].Value).ShouldBe(found);
            int.Parse(match.Groups[2].Value).ShouldBe(deleted);
            int.Parse(match.Groups[3].Value).ShouldBe(errors);

            return result;
        }

        public static IEnumerable<string> GetFiles(this OutputDirectory outDir, bool recurse = true)
        {
            var pattern = recurse ? "**/*.*" : "*.*";
            var wrapper = outDir.Path.CreateDirectorWrapper();

            var matcher = new Matcher().AddInclude(pattern);

            return matcher.Execute(wrapper).Files.Select(m => m.Path);
        }

        public static void VerifyFiles(this OutputDirectory outDir, int binFileCount, int objFileCount)
        {
            var wrapper = outDir.Path.CreateDirectorWrapper();

            var binMatcher = new Matcher().AddInclude("**/bin/**/*.*");
            var objMatcher = new Matcher().AddInclude("**/obj/**/*.*");

            var binFiles = binMatcher.Execute(wrapper).Files;
            binFiles.Count().ShouldBe(binFileCount);

            var objFiles = objMatcher.Execute(wrapper).Files;
            objFiles.Count().ShouldBe(objFileCount);
        }

        public static bool ContainsSet<T>(this IEnumerable<T> values, IEnumerable<T> others, IEqualityComparer<T> comparer)
        {
            foreach (var other in others)
            {
                if (!values.Contains(other, comparer))
                    return false;
            }

            return true;
        }

        private static DirectoryInfoWrapper CreateDirectorWrapper(this string path) =>
            new DirectoryInfoWrapper(new DirectoryInfo(path));
    }
}
