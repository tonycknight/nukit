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
        public void FindGlobbedDirectories_DiirectoryDoesNotExist_ReturnsEmpty(string path, string pattern)
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
        public void FindGlobbedDirectories_DiirectoryExists_ReturnsEmpty(string path, string pattern)
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
        public void FindGlobbedDirectories_DiirectoryExists_ReturnsFiles(string path, string pattern, params string[] files)
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

        [Theory]
        [InlineData("path", "bin", "a.txt")]
        public void FindBinaryDirectories_DiirectoryExists_ReturnsFiles(string path, string subPath, params string[] files)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetGetDirectories(path, [subPath]);

            var dirs = TestUtils.CreateDirectoryProvider()
                .SetDirectoryExists(path, true)
                .SetDirectoryExists(subPath, true)
                .SetGetDirectories(path, [], [])
                .SetGetDirectories(subPath, [], files);

            var finder = new DirectoryFinder(fs, dirs);

            var result = finder.FindBinaryDirectories(path).ToList();

            result.ShouldNotBeEmpty();
            result.Count.ShouldBe(1);
            result.All(r => r.EndsWith(subPath)).ShouldBeTrue();
        }

        [Theory]
        [InlineData("path", "obj", "a.txt")]
        public void FindObjectDirectories_DiirectoryExists_ReturnsFiles(string path, string subPath, params string[] files)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetGetDirectories(path, [subPath]);

            var dirs = TestUtils.CreateDirectoryProvider()
                .SetDirectoryExists(path, true)
                .SetDirectoryExists(subPath, true)
                .SetGetDirectories(path, [], [])
                .SetGetDirectories(subPath, [], files);

            var finder = new DirectoryFinder(fs, dirs);

            var result = finder.FindObjectDirectories(path).ToList();

            result.ShouldNotBeEmpty();
            result.Count.ShouldBe(1);
            result.All(r => r.EndsWith(subPath)).ShouldBeTrue();
        }
    }
}
