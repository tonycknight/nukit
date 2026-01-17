using FsCheck.Xunit;
using Nukit.FileSystem;

namespace Nukit.Tests.Unit.FileSystem
{
    public class FilePurgeInfoTests
    {
        [Property]
        internal bool Add_Right_AddedTo_Left(FilePurgeInfo left, FilePurgeInfo right)
        {
            var r = left.Add(right);

            return r.Found == (left.Found + right.Found)
                && r.Deleted == (left.Deleted + right.Deleted)
                && r.Errors.SequenceEqual(left.Errors.Concat(right.Errors));
        }

        [Property]
        internal bool Add_IntegerAdditions_AreCommutative(FilePurgeInfo left, FilePurgeInfo right)
        {
            var r1 = left.Add(right);
            var r2 = right.Add(left);

            return r1.Found == r2.Found
                && r1.Deleted == r2.Deleted
                && r1.Errors.Count == r2.Errors.Count;
        }

        [Property(MaxTest = 1000)]
        internal bool Add_ErrorMerge_IsNotCommutative(FilePurgeInfo left, FilePurgeInfo right, Guid[] leftErrors, Guid[] rightErrors)
        {
            if (leftErrors.Length == 0 || rightErrors.Length == 0)
                return true;

            left = left with { Errors = leftErrors.Select(g => g.ToString()).ToList() };
            right = right with { Errors = rightErrors.Select(g => g.ToString()).ToList() };

            var r1 = left.Add(right);
            var r2 = right.Add(left);

            return !r1.Errors.SequenceEqual(r2.Errors.AsEnumerable());
        }
    }
}
