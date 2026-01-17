using Nukit.FileSystem;
using Shouldly;

namespace Nukit.Tests.Unit.FileSystem
{
    public class FilePurgerTests
    {
        [Theory]
        [InlineData("c:\\")]
        [InlineData("c:\\abc")]
        [InlineData("/abc")]
        public void Delete_NoDryRun_DirectoryDoesNotExist_ReturnsEmpty(string path)
        {
            
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, false);

            var purger = new FilePurger(fs);

            DirectoryInfo directory = new(path);
            var result = purger.Delete(directory, false);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(0);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();            
        }

        [Theory]
        [InlineData("c:\\")]
        [InlineData("c:\\abc")]
        [InlineData("/abc")]
        public void Delete_DryRun_DirectoryDoesNotExist_ReturnsEmpty(string path)
        {

            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, false);

            var purger = new FilePurger(fs);

            DirectoryInfo directory = new(path);
            var result = purger.Delete(directory, true);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(0);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();            
        }

        [Theory]
        [InlineData("/test")]
        public void Delete_NoDryRun_NoFilesReturned_ReturnsEmpty(string path)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, true)
                .SetDirectoryGetFiles(path, [])
                .SetFileDelete(path, null);

            var purger = new FilePurger(fs);

            DirectoryInfo directory = new(path);
            var result = purger.Delete(directory, false);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(0);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("/test")]
        public void Delete_DryRun_NoFilesReturned_ReturnsEmpty(string path)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, true)
                .SetDirectoryGetFiles(path, [])
                .SetFileDelete(path, null);

            var purger = new FilePurger(fs);

            DirectoryInfo directory = new(path);
            var result = purger.Delete(directory, true);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(0);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();
        }

        [Theory]
        [InlineData("/test")]
        [InlineData("/test", "a.txt")]
        [InlineData("/test", "a.txt", "b.txt")]
        public void Delete_NoDryRun_FilesReturned_ReturnsFilesDeleted(string path, params string[] files)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, true)
                .SetDirectoryGetFiles(path, files)
                .SetFileDelete(path, null);

            var purger = new FilePurger(fs);

            DirectoryInfo directory = new(path);
            var result = purger.Delete(directory, false);

            result.Deleted.ShouldBe(files.Length);
            result.Found.ShouldBe(files.Length);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();
        }
    }
}
