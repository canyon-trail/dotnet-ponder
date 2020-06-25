using FluentAssertions;
using Xunit;

namespace Ponder.Tests
{
    public sealed class RelPathTests
    {
        [Theory]
        [InlineData("folder1", "folder2", "folder1\\folder2")]
        public void RelativeTo(string origin, string rel, string expected)
        {
            var originPath = new RelPath(origin);

            var relativePath = new RelPath(rel);

            var result = relativePath.RelativeTo(originPath);

            result.Path.Should().Be(expected);
        }
    }
}