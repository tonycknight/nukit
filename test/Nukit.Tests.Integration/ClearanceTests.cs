using FsCheck.Xunit;
using Shouldly;
using Xunit.Abstractions;

namespace Nukit.Tests.Integration
{
    public class ClearanceTests(ITestOutputHelper output)
    {
        [Property(MaxTest = 10)]
        public bool Clear_DryRun_FindsFiles_DoesNotDelete(string value)
        {
            using var outDir = TestUtils.GetOutDir();

            var prepResults = new[] {
                outDir.CreateProjectCommand(),
                outDir.AddGoodHttpPackageACommand(),
                outDir.DotnetBuildCommand(),
            }.Execute(output, true);

            var nukitResult = outDir.NukitCommand(true).Execute(output, true);

            nukitResult.OutLog.Contains("Nuke summary: Found: ").ShouldBeTrue();
            nukitResult.OutLog.Contains("Found: 0").ShouldBeFalse();
            nukitResult.OutLog.Contains("Deleted: 0").ShouldBeTrue();
            nukitResult.OutLog.Contains("Erros: 0").ShouldBeTrue();

            return true;
        }
    }
}
