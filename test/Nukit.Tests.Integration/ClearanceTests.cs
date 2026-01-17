using FsCheck.Xunit;
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

            nukitResult.VerifyNukitSummary(21, 0, 0);

            return true;
        }
    }
}
