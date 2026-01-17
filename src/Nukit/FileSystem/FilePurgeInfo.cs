namespace Nukit.FileSystem
{
    internal record FilePurgeInfo
    {
        public string Directory { get; init; } = "";
        public int Found { get; init; }
        public int Deleted { get; init; }
        public List<string> Errors { get; init; } = new();
    }

    internal static class FilePurgeInfoExtensions
    {
        public static FilePurgeInfo Add(this FilePurgeInfo value, FilePurgeInfo second) => value with
        {
            Found = value.Found + second.Found,
            Deleted = value.Deleted + second.Deleted,
            Errors = value.Errors.Concat(second.Errors).ToList(),
        };
    }
}
