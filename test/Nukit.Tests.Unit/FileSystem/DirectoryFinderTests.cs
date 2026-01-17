using Nukit.FileSystem;
using Shouldly;

namespace Nukit.Tests.Unit.FileSystem
{
    public class DirectoryFinderTests
    {
        [Theory]
        [InlineData("", "")]
        [InlineData("", "**")]
        [InlineData("/path", "**")]
        public void DirectoryFinder_FindGlobbedDirectories_DiirectoryDoesNotExist_ReturnsEmpty(string path, string pattern)
        {
            var fs = TestUtils.CreateFileSystem();

            var dirs = TestUtils.CreateDirectoryProvider()
                .SetDirectoryExists(path, false);

            var finder = new DirectoryFinder(fs, dirs);

            var result = finder.FindGlobbedDirectories(path, pattern);

            result.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("", "**")]
        [InlineData("/path", "**")]
        public void DirectoryFinder_FindGlobbedDirectories_DiirectoryExists_ReturnsEmpty(string path, string pattern)
        {
            var fs = TestUtils.CreateFileSystem();

            var dirs = TestUtils.CreateDirectoryProvider()
                .SetDirectoryExists(path, true)
                .SetGetDirectories(path, [pattern], []);

            var finder = new DirectoryFinder(fs, dirs);

            var result = finder.FindGlobbedDirectories(path, pattern);

            result.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("path", "**", "a.txt")]
        public void DirectoryFinder_FindGlobbedDirectories_DiirectoryExists_ReturnsFiles(string path, string pattern, params string[] files)
        {
            var fs = TestUtils.CreateFileSystem();

            var dirs = TestUtils.CreateDirectoryProvider()
                .SetDirectoryExists(path, true)
                .SetGetDirectories(path, [pattern], files);

            var finder = new DirectoryFinder(fs, dirs);

            var result = finder.FindGlobbedDirectories(path, pattern).ToList();

            result.ShouldNotBeEmpty();
            result.Count.ShouldBe(files.Length);
            result.Zip(files).All(t => t.First.EndsWith(path)).ShouldBeTrue();
        }
    }
}
