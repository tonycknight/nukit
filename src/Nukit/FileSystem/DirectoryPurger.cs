using System.IO.Abstractions;
using Polly;
using Polly.Retry;

namespace Nukit.FileSystem
{
    internal interface IDirectoryPurger
    {
        FilePurgeInfo Delete(string directory, bool dryRun, int retries);
    }

    internal class DirectoryPurger(IFileSystem fs) : IDirectoryPurger
    {
        public FilePurgeInfo Delete(string directory, bool dryRun, int retries)
        {
            int found = 0;
            int deleted = 0;
            var errors = new List<string>();

            if (fs.Directory.Exists(directory))
            {
                var resilience = CreateResilienceStrategy(retries, TimeSpan.FromSeconds(5));

                var files = fs.Directory.GetFiles(directory, "*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    found++;
                    if (!dryRun)
                    {
                        try
                        {
                            resilience.Execute(() => fs.File.Delete(file));
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
                        resilience.Execute(() => fs.Directory.Delete(directory, true));
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
                    }
                }
            }

            return new FilePurgeInfo { Deleted = deleted, Found = found, Directory = directory, Errors = errors };
        }

        private ResiliencePipeline CreateResilienceStrategy(int retries, TimeSpan timeout)
        {
            var builder = new ResiliencePipelineBuilder();

            if (retries > 0)
            {
                var options = new RetryStrategyOptions()
                {
                    ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    MaxRetryAttempts = retries > 0 ? retries : 0,
                    Delay = TimeSpan.FromMilliseconds(100),
                };

                builder
                    .AddRetry(options)
                    .AddTimeout(timeout);
            }

            return builder.Build();
        }
    }
}
