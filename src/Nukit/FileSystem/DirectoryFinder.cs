using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Nukit.FileSystem
{
    internal interface IDirectoryFinder
    {
        IEnumerable<string> GetDirectories(string path, string[]? includedPaths, string[]? excludedPaths);
    }

    [ExcludeFromCodeCoverage] // The Match class cannot accept injected IFileSystem instances
    internal class DirectoryFinder : IDirectoryFinder
    {
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

        private static DirectoryInfoWrapper CreateDirectoryWrapper(string path) => new(new DirectoryInfo(path));
    }
}
