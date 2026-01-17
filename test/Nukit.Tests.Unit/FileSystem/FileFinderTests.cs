using Nukit.FileSystem;
using Shouldly;

namespace Nukit.Tests.Unit.FileSystem
{
    public class FileFinderTests
    {
        [Fact]
        public void FindGlobbedDirectories_TODO()
        {
            string path = "";
            string pattern = "";

            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, true);

            var finder = new FileFinder(fs);
                        
            // TODO: 
            //var results = finder.FindGlobbedDirectories(path, pattern).ToList();
        }

    }
}
