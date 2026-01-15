using FsCheck.Xunit;
using Shouldly;

namespace Nukit.Tests.Unit
{
    public class ExtensionsTests
    {
        // TODO: temporary
        [Property]
        public bool ToTaskResult(string value)
        {
            var result = value.ToTaskResult();

            result.ShouldNotBeNull();

            return true;
        }
    }
}
