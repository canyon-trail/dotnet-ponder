using System.IO;
using FluentAssertions;
using Xunit;

namespace Ponder.Tests
{
    public sealed class ProjectTests
    {
        [Fact]
        public void DefaultFilePaths()
        {
            var testee = new Project(
                Path.Combine(".", "somewhere", "sample.csproj")
                );

            var expected = new[] {
                testee.CsProjPath,
                Path.Combine(".", "somewhere", "**", "*.cs")
            };

            testee.WatchPaths.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void WatchFolder()
        {
            var testee = new Project(
                Path.Combine(".", "somewhere", "sample.csproj")
                );

            var expected = Path.Combine(".", "somewhere");

            testee.WatchFolder.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, "somewhere", "foo", "bar.cs")]
        [InlineData(false, "somewhere", "foo", "bar.png")]
        [InlineData(false, "elsewhere", "foo", "bar.cs")]
        [InlineData(true, "somewhere", "sample.csproj")]
        public void CsFileMatches(bool isMatch, params string[] segments)
        {
            var fullPath = Path.Combine(".", Path.Combine(segments));

            var testee = new Project(
                Path.Combine(".", "somewhere", "sample.csproj")
                );

            testee.IsMatch(fullPath).Should().Be(isMatch);
        }
    }
}