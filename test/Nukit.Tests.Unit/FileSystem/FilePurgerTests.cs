using Nukit.FileSystem;
using Shouldly;

namespace Nukit.Tests.Unit.FileSystem
{
    public class FilePurgerTests
    {
        [Theory]
        [InlineData("c:\\")]
        [InlineData("c:\\abc")]
        [InlineData("\\abc")]
        public void Delete_DirectoryDoesNotExist_ReturnsUnDeleted(string path)
        {
            
            var fs = TestUtils.CreateFileSystem()
                .SetDirectoryExists(path, false);

            var purger = new FilePurger(fs);

            DirectoryInfo directory = new(path);
            var result = purger.Delete(directory, false);

            result.Deleted.ShouldBe(0);
            result.Found.ShouldBe(0);
            result.Errors.ShouldBeEmpty();
            result.Directory.ShouldNotBeNullOrWhiteSpace();
            result.Directory.ShouldContain(path);            
        }
    }
}
