using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Nukit.FileSystem
{
    internal interface IDirectoryFinder
    {
        bool DirectoryExists(string path);
        IEnumerable<string> GetDirectories(string path, string[]? includedPaths, string[]? excludedPaths);
    }

    [ExcludeFromCodeCoverage] // The Match class cannot accept injected IFileSystem instances
    internal class DirectoryFinder(IFileSystem fs) : IDirectoryFinder
    {
        public bool DirectoryExists(string path) => fs.DirectoryInfo.New(path).Exists;

        public IEnumerable<string> GetDirectories(string path, string[]? includedPaths, string[]? excludedPaths)
        {
            var matcher = Create(includedPaths, excludedPaths);

            var dir = CreateDirectoryWrapper(path);

            return matcher.Execute(dir).Files.Select(m => m.Path);
        }

        private Matcher Create(string[]? includedPaths, string[]? excludedPaths)
        {
            var result = new Matcher();
            if(includedPaths != null)
                result.AddIncludePatterns(includedPaths);
            if(excludedPaths != null)
                result.AddExcludePatterns(excludedPaths);
            return result;
        }

        private static Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper CreateDirectoryWrapper(string path) => new(new DirectoryInfo(path));
    }
}
