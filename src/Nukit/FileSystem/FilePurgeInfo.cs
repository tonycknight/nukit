using System;
using System.Collections.Generic;
using System.Text;

namespace Nukit.FileSystem
{
    internal record FilePurgeInfo
    {
        public DirectoryInfo Directory { get; init; }
        public int Found { get; init; }
        public int Deleted { get; init; }
        public int Errors { get; init; }
    }

    internal static class FilePurgeInfoExtensions
    {
        public static FilePurgeInfo Add(this FilePurgeInfo value, FilePurgeInfo second) => value with
        {
            Found = value.Found + second.Found,
            Deleted = value.Deleted + second.Deleted,
            Errors = value.Errors + second.Errors
        };
    }
}
