using NSubstitute;
using Nukit.FileSystem;
using Shouldly;

namespace Nukit.Tests.Unit.FileSystem
{
    public class DirectoryPurgerTests
    {
        [Theory]
        [InlineData("c:\\")]
        [InlineData("c:\\abc")]
        [InlineData("/abc")]
        public void Delete_NoDryRun_DirectoryDoesNotExist_ReturnsEmpty(string path)
        {

            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, false);

            var purger = new DirectoryPurger(fs);

            var result = purger.Delete(path, false);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(0);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();
            fs.Directory.Received(0).Delete(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Theory]
        [InlineData("c:\\")]
        [InlineData("c:\\abc")]
        [InlineData("/abc")]
        public void Delete_DryRun_DirectoryDoesNotExist_ReturnsEmpty(string path)
        {

            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, false);

            var purger = new DirectoryPurger(fs);

            var result = purger.Delete(path, true);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(0);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();
            fs.Directory.Received(0).Delete(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Theory]
        [InlineData("/test")]
        public void Delete_NoDryRun_NoFilesReturned_ReturnsEmpty(string path)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, true)
                .SetDirectoryGetFiles(path, [])
                .SetFileDelete(path, null);

            var purger = new DirectoryPurger(fs);

            var result = purger.Delete(path, false);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(0);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();
            fs.Directory.Received(1).Delete(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Theory]
        [InlineData("/test")]
        public void Delete_DryRun_NoFilesReturned_ReturnsEmpty(string path)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, true)
                .SetDirectoryGetFiles(path, [])
                .SetFileDelete(path, null);

            var purger = new DirectoryPurger(fs);

            var result = purger.Delete(path, true);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(0);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();
            fs.Directory.Received(0).Delete(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Theory]
        [InlineData("c:\\test")]
        [InlineData("c:\\test", "a.txt")]
        [InlineData("c:\\test", "a.txt", "b.txt")]
        public void Delete_NoDryRun_FilesReturned_ReturnsFilesDeleted(string path, params string[] files)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, true)
                .SetDirectoryGetFiles(path, files)
                .SetFileDelete(path, null);

            var purger = new DirectoryPurger(fs);

            var result = purger.Delete(path, false);

            result.Deleted.ShouldBe(files.Length);
            result.Found.ShouldBe(files.Length);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();

            fs.File.Received(files.Length).Delete(Arg.Any<string>());
            fs.Directory.Received(1).Delete(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Theory]
        [InlineData("/test")]
        [InlineData("/test", "a.txt")]
        [InlineData("/test", "a.txt", "b.txt")]
        public void Delete_DryRun_FilesReturned_ReturnsFilesDeleted(string path, params string[] files)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, true)
                .SetDirectoryGetFiles(path, files)
                .SetFileDelete(path, null);

            var purger = new DirectoryPurger(fs);

            var result = purger.Delete(path, true);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(files.Length);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();

            fs.File.Received(0).Delete(Arg.Any<string>());
            fs.Directory.Received(0).Delete(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Theory]
        [InlineData("c:\\test")]
        [InlineData("c:\\test", "a.txt")]
        [InlineData("c:\\test", "a.txt", "b.txt")]
        public void Delete_NoDryRun_ErrorInDeletion_ReturnsNoFilesDeleted(string path, params string[] files)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, true)
                .SetDirectoryGetFiles(path, files)
                .SetFileDelete(path, new InvalidOperationException());

            var purger = new DirectoryPurger(fs);

            var result = purger.Delete(path, false);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(files.Length);
            result.Errors.Count.ShouldBe(files.Length);
            result.Directory.ShouldNotBeNullOrWhiteSpace();

            fs.File.Received(files.Length).Delete(Arg.Any<string>());
            fs.Directory.Received(1).Delete(Arg.Any<string>(), Arg.Any<bool>());
        }

        [Theory]
        [InlineData("c:\\test")]
        [InlineData("c:\\test", "a.txt")]
        [InlineData("c:\\test", "a.txt", "b.txt")]
        public void Delete_NoDryRun_ErrorInDirectoryDeletion_ReturnsFilesDeleted(string path, params string[] files)
        {
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, true)
                .SetDirectoryGetFiles(path, files)
                .SetFileDelete(path, null)
                .SetDirectoryDelete(path, new InvalidOperationException());

            var purger = new DirectoryPurger(fs);

            var result = purger.Delete(path, false);

            result.Deleted.ShouldBe(files.Length);
            result.Found.ShouldBe(files.Length);
            result.Errors.Count.ShouldBe(1);
            result.Directory.ShouldNotBeNullOrWhiteSpace();

            fs.File.Received(files.Length).Delete(Arg.Any<string>());
            fs.Directory.Received(1).Delete(Arg.Any<string>(), Arg.Any<bool>());
        }
    }
}
