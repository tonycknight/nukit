using System.IO.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Nukit.FileSystem
{
    internal interface IFileFinder
    {
        string Normalise(string path);
        IEnumerable<DirectoryInfo> FindBinaryDirectories(string path);
        IEnumerable<DirectoryInfo> FindObjectDirectories(string path);
        IEnumerable<DirectoryInfo> FindTestResultDirectories(string path);
    }

    internal class FileFinder(IFileSystem fs) : IFileFinder
    {
        private readonly Matcher _binMatcher = CreateBinMatcher();
        private readonly Matcher _objMatcher = CreateObjectMatcher();
        private readonly Matcher _trxMatcher = CreateTestResultMatcher();

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

        public IEnumerable<DirectoryInfo> FindBinaryDirectories(string path)
        {
            var dirs = fs.Directory.GetDirectories(path, "bin", SearchOption.AllDirectories);

            return dirs.Where(d => IsDirectoryMatch(d, _binMatcher)).Select(p => new DirectoryInfo(p));
        }

        public IEnumerable<DirectoryInfo> FindObjectDirectories(string path)
        {
            var dirs = fs.Directory.GetDirectories(path, "obj", SearchOption.AllDirectories);

            return dirs.Where(d => IsDirectoryMatch(d, _objMatcher)).Select(p => new DirectoryInfo(p));
        }

        public IEnumerable<DirectoryInfo> FindTestResultDirectories(string path)
        {
            var dirs = fs.Directory.GetDirectories(path, "TestResults", SearchOption.AllDirectories);

            return dirs.Where(d => IsDirectoryMatch(d, _trxMatcher)).Select(p => new DirectoryInfo(p));
        }

        private bool IsDirectoryMatch(string path, Matcher matcher)
        {
            var di = fs.DirectoryInfo.New(path);
            if (di.Exists)
            {
                var wrapper = new Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper(new DirectoryInfo(path));

                return matcher.Execute(wrapper).HasMatches;
            }

            return false;
        }

        private static Matcher CreateBinMatcher()
        {
            var result = new Matcher();
            result.AddInclude("**/*.dll");
            return result;
        }

        private static Matcher CreateObjectMatcher()
        {
            var result = new Matcher();
            result.AddInclude("**/project.assets.json");
            return result;
        }

        private static Matcher CreateTestResultMatcher()
        {
            var result = new Matcher();
            result.AddInclude("**/*.trx");
            return result;
        }
    }
}
