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
            fs.DirectoryInfo.New(path).Exists.Returns(exists);
            return fs;
        }
    }
}
