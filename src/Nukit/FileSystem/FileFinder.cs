using System.IO.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Nukit.FileSystem
{
    internal interface IFileFinder
    {
        string Normalise(string path);
        IEnumerable<DirectoryInfo> FindBinaryDirectories(string path);
        IEnumerable<DirectoryInfo> FindObjectDirectories(string path);
        IEnumerable<DirectoryInfo> FindGlobbedDirectories(string path, string pattern);
    }

    internal class FileFinder(IFileSystem fs) : IFileFinder
    {
        private readonly Matcher _binMatcher = CreateMatcher("**/*.dll");
        private readonly Matcher _objMatcher = CreateMatcher("**/project.assets.json");

        public string Normalise(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                var wd = fs.Directory.GetCurrentDirectory();
                path = Path.Combine(wd, path);
                return Path.GetFullPath(path);
            }
            return Path.GetFullPath(path);
        }

        public IEnumerable<DirectoryInfo> FindBinaryDirectories(string path) => FindDirectories(path, "bin", _binMatcher);

        public IEnumerable<DirectoryInfo> FindObjectDirectories(string path) => FindDirectories(path, "obj", _objMatcher);

        public IEnumerable<DirectoryInfo> FindGlobbedDirectories(string path, string pattern)
        {
            var matcher = new Matcher();
            matcher.AddInclude(pattern);

            return GetDirectoryMatches(path, matcher).Select(p => new DirectoryInfo(p));
        }

        private IEnumerable<DirectoryInfo> FindDirectories(string path, string pattern, Matcher matcher)
        {
            var dirs = fs.Directory.GetDirectories(path, pattern, SearchOption.AllDirectories);

            return dirs.Where(d => IsDirectoryMatch(d, matcher)).Select(p => new DirectoryInfo(p));
        }

        private bool IsDirectoryMatch(string path, Matcher matcher)
        {
            if (fs.DirectoryInfo.New(path).Exists)
            {
                var wrapper = CreateDirectoryWrapper(path);

                return matcher.Execute(wrapper).HasMatches;
            }

            return false;
        }

        private IEnumerable<string> GetDirectoryMatches(string path, Matcher matcher)
        {
            if (fs.DirectoryInfo.New(path).Exists)
            {
                var wrapper = CreateDirectoryWrapper(path);

                return matcher.Execute(wrapper).Files
                    .Select(f => Path.Combine(path, f.Path))
                    .Select(f => Path.GetDirectoryName(f) ?? String.Empty)
                    .Where(d => d != String.Empty)
                    .Distinct();
            }

            return Enumerable.Empty<string>();
        }

        private static Matcher CreateMatcher(string pattern)
        {
            var result = new Matcher();
            result.AddInclude(pattern);
            return result;
        }

        private static Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper CreateDirectoryWrapper(string path) => new(new DirectoryInfo(path));
    }
}
