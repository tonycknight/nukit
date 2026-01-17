using FsCheck;
using FsCheck.Xunit;
using Shouldly;

namespace Nukit.Tests.Unit
{
    public class ExtensionsTests
    {
        [Property]
        public bool ToTaskResult(string value)
        {
            var result = value.ToTaskResult();

            result.ShouldNotBeNull();

            return true;
        }

        [Fact]
        public void Coalesce_NullSequence_ReturnsEmpty()
        {
            int[]? values = null;
            var result = values.Coalesce();

            result.ShouldBe(Enumerable.Empty<int>());
        }

        [Theory]
        [InlineData([])]
        [InlineData([1])]
        [InlineData([1, 2, 3])]
        public void Coalesce_NonNullSequence_ReturnsEmpty(params int[]? values)
        {
            var result = values.Coalesce();

            result.ShouldBe(values);
        }

        [Property]
        public bool Indent_StringIsIntendedX(PositiveInt indentation)
        {
            var value = "abc";

            var result = value.Indent(indentation.Get);

            return result.EndsWith(value)
                && result.Length == indentation.Get + value.Length
                && result.Trim() == value;
        }
    }
}
