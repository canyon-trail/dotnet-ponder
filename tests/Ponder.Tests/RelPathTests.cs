using System.Linq;
using FluentAssertions;
using Xunit;

namespace Ponder.Tests
{
    public sealed class RelPathTests
    {
        [Theory]
        [InlineData("folder1", "folder2", "folder1\\folder2")]
        [InlineData("folder1", "..", ".")]
        [InlineData("folder1\\folder2", "..\\..", ".")]
        public void RelativeTo(string origin, string rel, string expected)
        {
            var originPath = RelPath.FromString(origin);

            var relativePath = RelPath.FromString(rel);

            var result = relativePath.RelativeTo(originPath);

            result.Path.Should().Be(expected);
        }

        [Theory]
        [InlineData("folder1\\..", ".")]
        [InlineData(".\\folder1\\..", ".")]
        [InlineData(".\\.\\folder1\\..", ".")]
        public void FromString(string input, string expected)
        {
            var result = RelPath.FromString(input);

            result.Path.Should().Be(expected);
        }
    }
}