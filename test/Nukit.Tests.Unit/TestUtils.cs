using System.IO;
using System.IO.Abstractions;
using NSubstitute;

namespace Nukit.Tests.Unit
{
    internal static class TestUtils
    {
        public static IFileSystem CreateFileSystem()
        {
            var dir = Substitute.For<IDirectory>();
            var files = Substitute.For<IFile>();

            var fs = Substitute.For<IFileSystem>();
            fs.Directory.Returns(dir);
            fs.File.Returns(files);

            return fs;
        }

        public static IFileSystem SetCurrentDirectory(this IFileSystem fs, string path)
        {
            fs.Directory.GetCurrentDirectory().Returns(path);

            return fs;
        }

        public static IFileSystem SetDirectoryExists(this IFileSystem fs, string path, bool exists = true)
        {
            fs.Directory.Exists(Arg.Is(path)).Returns(exists);
            return fs;
        }

        public static IFileSystem SetDirectoryGetFiles(this IFileSystem fs, string path, string[] files)
        {
            fs.Directory.GetFiles(Arg.Is(path), Arg.Is("*"), Arg.Is(SearchOption.AllDirectories))
                .Returns(files);

            return fs;
        }

        public static IFileSystem SetFileDelete(this IFileSystem fs, string path, Exception? exception = null)
        {
            if (exception != null)
                fs.File.When(f => f.Delete(Arg.Any<string>())).Throws(exception);

            return fs;
        }

        public static IFileSystem SetDirectoryDelete(this IFileSystem fs, string path, Exception? exception = null)
        {
            if (exception != null)
                fs.Directory.When(f => f.Delete(Arg.Any<string>(), Arg.Any<bool>())).Throws(exception);

            return fs;
        }
    }
}
