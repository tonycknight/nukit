using System.IO.Abstractions;

namespace Nukit.FileSystem
{
    internal interface IFilePurger
    {
        FilePurgeInfo Delete(string directory, bool dryRun);
    }

    internal class FilePurger(IFileSystem fs) : IFilePurger
    {
        public FilePurgeInfo Delete(string directory, bool dryRun)
        {
            int found = 0;
            int deleted = 0;
            var errors = new List<string>();

            if (fs.Directory.Exists(directory))
            {
                var files = fs.Directory.GetFiles(directory, "*", SearchOption.AllDirectories);

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
                        catch (Exception ex)
                        {
                            errors.Add(ex.Message);
                        }
                    }
                }

                if (!dryRun)
                {
                    try
                    {
                        fs.Directory.Delete(directory, true);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                }
            }

            return new FilePurgeInfo { Deleted = deleted, Found = found, Directory = directory, Errors = errors };
        }
    }
}
