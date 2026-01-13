using System.IO.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Nukit.FileSystem
{
    internal interface IFileFinder
    {
        IEnumerable<DirectoryInfo> FindBinaryDirectories(string path);
    }

    internal class FileFinder(IFileSystem fs) : IFileFinder
    {
        private readonly Matcher _binMatcher = CreateBinMatcher();
        
        public IEnumerable<DirectoryInfo> FindBinaryDirectories(string path)
        {
            var dirs = fs.Directory.GetDirectories(path, "bin", SearchOption.AllDirectories);

            return dirs.Where(IsBinaryMatch).Select(p => new DirectoryInfo(p));
        }

        private bool IsBinaryMatch(string path)
        {            
            var di = fs.DirectoryInfo.New(path);

            if (di.Exists)
            {
                var wrapper = new Microsoft.Extensions.FileSystemGlobbing.Abstractions.DirectoryInfoWrapper(new DirectoryInfo(path));                

                var matches = _binMatcher.Execute(wrapper);

                return matches.HasMatches;
            }

            return false;
        }

        private static Matcher CreateBinMatcher()
        {
            var result = new Matcher();
            result.AddInclude("**/*.dll");
            return result;
        }
        // project.assets.json
    }
}
