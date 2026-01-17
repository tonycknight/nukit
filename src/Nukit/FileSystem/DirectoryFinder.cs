using System.IO.Abstractions;

namespace Nukit.FileSystem
{
    internal interface IDirectoryFinder
    {
        IEnumerable<DirectoryInfo> FindBinaryDirectories(string path);
        IEnumerable<DirectoryInfo> FindObjectDirectories(string path);
        IEnumerable<DirectoryInfo> FindGlobbedDirectories(string path, string pattern);
    }

    internal class DirectoryFinder(IFileSystem fs, IDirectoryProvider directories) : IDirectoryFinder
    {
        public IEnumerable<DirectoryInfo> FindBinaryDirectories(string path) => FindDirectories(path, "bin", ["**/*.dll"]);

        public IEnumerable<DirectoryInfo> FindObjectDirectories(string path) => FindDirectories(path, "obj", ["**/project.assets.json"]);

        public IEnumerable<DirectoryInfo> FindGlobbedDirectories(string path, string pattern) =>
            GetDirectoryMatches(path, [pattern]).Select(p => new DirectoryInfo(p));

        private IEnumerable<DirectoryInfo> FindDirectories(string path, string pattern, string[] includedPaths)
        {
            var dirs = fs.Directory.GetDirectories(path, pattern, SearchOption.AllDirectories);

            return dirs.Where(d => IsDirectoryMatch(d, includedPaths)).Select(p => new DirectoryInfo(p));
        }

        private bool IsDirectoryMatch(string path, string[] includedPaths)
        {
            if (directories.DirectoryExists(path))
            {
                return directories.GetDirectories(path, includedPaths, null).Any();
            }

            return false;
        }

        private IEnumerable<string> GetDirectoryMatches(string path, string[] includedPaths)
        {
            if (directories.DirectoryExists(path))
            {
                return directories.GetDirectories(path, includedPaths, null)
                    .Select(p => Path.Combine(path, p))
                    .Select(f => Path.GetDirectoryName(f) ?? String.Empty)
                    .Where(d => d != String.Empty)
                    .Distinct();
            }

            return Enumerable.Empty<string>();
        }
    }
}
