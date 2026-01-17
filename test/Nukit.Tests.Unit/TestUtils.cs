using System.IO;
using System.IO.Abstractions;
using NSubstitute;

namespace Nukit.Tests.Unit
{
    internal static class TestUtils
    {
        public static IFileSystem CreateFileSystem() => Substitute.For<IFileSystem>();

        public static IFileSystem SetCurrentDirectory(this IFileSystem fs, string path)
        {
            fs.Directory.GetCurrentDirectory().Returns(path);

            return fs;
        }

        public static IFileSystem SetDirectoryExists(this IFileSystem fs, string path, bool exists = true)
        {
            fs.Directory.Exists(Arg.Any<string>()).Returns(exists);
            return fs;
        }

        public static IFileSystem SetDirectoryGetFiles(this IFileSystem fs, string path, string[] files)
        {
            fs.Directory.GetFiles(Arg.Any<string>(), Arg.Is("*"), Arg.Is(SearchOption.AllDirectories))
                .Returns(files);

            return fs;
        }

        public static IFileSystem SetFileDelete(this IFileSystem fs, string path, Exception? exception = null)
        {
            var files = Substitute.For<IFile>();
            fs.File.Returns(files);
            files.When(f => f.Delete(Arg.Is<string>(path)))
                .Throws(exception);

            return fs;
        }
    }
}
