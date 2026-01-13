using System.IO.Abstractions;

namespace Nukit.FileSystem
{
    internal record FilePurgeInfo
    {
        public DirectoryInfo Directory { get; init; }
        public int Found { get; init; }
        public int Deleted { get; init; }
        public int Errors { get; init; }
    }

    internal interface IFilePurger
    {
        FilePurgeInfo Delete(DirectoryInfo directory, bool dryRun);
    }

    internal class FilePurger(IFileSystem fs) : IFilePurger
    {
        public FilePurgeInfo Delete(DirectoryInfo directory, bool dryRun)
        {
            int found = 0;
            int deleted = 0;
            int errors = 0;
            if (directory.Exists)
            {
                var files = fs.Directory.GetFiles(directory.FullName, "*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    found++;
                    if (!dryRun)
                    {
                        try
                        {
                            fs.File.Delete(file);
                            deleted++;
                        }
                        catch (Exception)
                        {
                            // TODO: log?
                            errors++;
                        }
                    }
                }

            }

            return new FilePurgeInfo { Deleted = deleted, Found = found, Directory = directory, Errors = errors };
        }
    }
}
