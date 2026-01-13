using System.IO.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Nukit.FileSystem
{
    internal interface IFileFinder
    {
        IEnumerable<DirectoryInfo> FindDirectories(string path, string searchPattern);
    }

    internal class FileFinder(IFileSystem fs) : IFileFinder
    {
        private readonly Matcher _matcher = CreateMatcher();
        
        public IEnumerable<DirectoryInfo> FindDirectories(string path, string searchPattern)
        {
            var dirs = fs.Directory.GetDirectories(path, searchPattern, SearchOption.AllDirectories);

            return dirs.Where(IsMatch).Select(p => new DirectoryInfo(p));
        }

        private bool IsMatch(string path)
        {            
            var di = fs.DirectoryInfo.New(path);

            if (di.Exists)
            {
                var wrapper = new Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper(new DirectoryInfo(path));                

                var matches = _matcher.Execute(wrapper);

                return matches.HasMatches;
            }

            return false;
        }

        private static Matcher CreateMatcher()
        {
            var result = new Matcher();
            result.AddInclude("**/*.dll");
            return result;
        }
    }
}
