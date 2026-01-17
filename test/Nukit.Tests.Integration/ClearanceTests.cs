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
            var sourceFiles = outDir.GetFiles(false).ToList();
            var files = outDir.GetFiles(true).ToList();

            var nukitResult = outDir.NukitCommand(dryRun: true, nukeBin: true, nukeObj: true, args: "").Execute(output, true);

            var postFiles = outDir.GetFiles(true).ToList();
            nukitResult.VerifyNukitSummary(postFiles.Count - sourceFiles.Count, 0, 0);
            outDir.GetBinaryFiles().Count().ShouldBeGreaterThan(0);
            outDir.GetObjectFiles().Count().ShouldBeGreaterThan(0);

            postFiles.ContainsSet(files, StringComparer.InvariantCultureIgnoreCase).ShouldBeTrue();
        }

        [Fact]
        public void Clear_Deletion_FindsFiles_NoObjectDeletion_DeletesFiles()
        {
            using var outDir = TestUtils.GetOutDir();

            outDir.CreateProjectCommand().Execute(output, true);
            var sourceFiles = outDir.GetFiles(false).ToList();
            var files = outDir.GetFiles(true).ToList();
            outDir.DotnetBuildCommand().Execute(output, true);
            var binFiles = outDir.GetBinaryFiles().ToList();
            var objFiles = outDir.GetObjectFiles().ToList();

            var nukitResult = outDir.NukitCommand(dryRun: false, nukeBin: true, nukeObj: false, args: "").Execute(output, true);

            var postFiles = outDir.GetFiles(true).ToList();

            nukitResult.VerifyNukitSummary(binFiles.Count, binFiles.Count, 0);
            outDir.GetBinaryFiles().Count().ShouldBe(0);
            outDir.GetObjectFiles().Count().ShouldBeGreaterThan(0);
            postFiles.ContainsSet(files, StringComparer.InvariantCultureIgnoreCase).ShouldBeTrue();
        }

        [Fact]
        public void Clear_Deletion_FindsFiles_ObjectDeletion_DeletesFiles()
        {
            using var outDir = TestUtils.GetOutDir();

            outDir.CreateProjectCommand().Execute(output, true);
            var files = outDir.GetFiles(false).ToList();
            outDir.DotnetBuildCommand().Execute(output, true);
            var binFiles = outDir.GetBinaryFiles().ToList();
            var objFiles = outDir.GetObjectFiles().ToList();

            var nukitResult = outDir.NukitCommand(dryRun: false, nukeBin: true, nukeObj: true, args: "").Execute(output, true);

            nukitResult.VerifyNukitSummary(binFiles.Count + objFiles.Count, binFiles.Count + objFiles.Count, 0);
            outDir.GetFiles(true).ContainsSet(files, StringComparer.InvariantCultureIgnoreCase).ShouldBeTrue();
        }

        [Theory]
        [InlineData("**")]
        [InlineData("**/bin/Release")]
        [InlineData("**/obj")]
        public void Clear_Deletion_FindsFiles_GlobDeletion_DeletesFiles(string glob)
        {
            using var outDir = TestUtils.GetOutDir();

            outDir.CreateProjectCommand().Execute(output, true);
            var files = outDir.GetFiles(false).ToList();
            outDir.DotnetBuildCommand().Execute(output, true);
            var binFiles = outDir.GetBinaryFiles().ToList();
            var objFiles = outDir.GetObjectFiles().ToList();
            var globFiles = outDir.GetFiles(glob).ToList();

            var nukitResult = outDir.NukitCommand(dryRun: false, nukeBin: false, nukeObj: false, args: $"--glob {glob}").Execute(output, true);

            var postFiles = outDir.GetFiles(glob).ToList();
            nukitResult.VerifyNukitSummary(globFiles.Count, globFiles.Count, 0);
            postFiles.ShouldBeEmpty();
        }
    }
}
