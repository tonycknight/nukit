using Shouldly;
using Xunit.Abstractions;

namespace Nukit.Tests.Integration
{
    public class ClearanceTests(ITestOutputHelper output)
    {
        [Fact]
        public void Clear_DryRun_FindsFiles_DoesNotDelete()
        {
            using var outDir = TestUtils.GetOutDir();

            new[] { outDir.CreateProjectCommand(), outDir.DotnetBuildCommand() }.Execute(output, true);
            var files = outDir.GetFiles(true).ToList();

            var nukitResult = outDir.NukitCommand(true).Execute(output, true);

            nukitResult.VerifyNukitSummary(21, 0, 0);
            outDir.VerifyFiles(3, 18);
            outDir.GetFiles(true).ContainsSet(files, StringComparer.InvariantCultureIgnoreCase).ShouldBeTrue();
        }

        [Fact]
        public void Clear_Deletion_FindsFiles_NoObjectDeletion_DeletesFiles()
        {
            using var outDir = TestUtils.GetOutDir();

            outDir.CreateProjectCommand().Execute(output, true);
            var files = outDir.GetFiles(true).ToList();
            outDir.DotnetBuildCommand().Execute(output, true);

            var nukitResult = outDir.NukitCommand(false, nukeBin: true, nukeObj: false).Execute(output, true);

            nukitResult.VerifyNukitSummary(3, 3, 0);
            outDir.VerifyFiles(0, 18);
            outDir.GetFiles(true).ContainsSet(files, StringComparer.InvariantCultureIgnoreCase).ShouldBeTrue();
        }

        [Fact]
        public void Clear_Deletion_FindsFiles_ObjectDeletion_DeletesFiles()
        {
            using var outDir = TestUtils.GetOutDir();

            outDir.CreateProjectCommand().Execute(output, true);
            var files = outDir.GetFiles(false).ToList();
            outDir.DotnetBuildCommand().Execute(output, true);

            var nukitResult = outDir.NukitCommand(false, nukeBin: true, nukeObj: true).Execute(output, true);

            nukitResult.VerifyNukitSummary(21, 21, 0);
            outDir.VerifyFiles(0, 0);
            outDir.GetFiles(true).ContainsSet(files, StringComparer.InvariantCultureIgnoreCase).ShouldBeTrue();
        }
    }
}
