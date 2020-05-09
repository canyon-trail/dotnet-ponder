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
        public void ProjectFolder()
        {
            var testee = new Project(
                Path.Combine(".", "somewhere", "sample.csproj")
                );

            var expected = Path.Combine(".", "somewhere");

            testee.ProjectFolder.Should().Be(expected);
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

        [Fact]
        public void TransitiveWatchFolders()
        {
            var referencedProject = new Project(
                Path.Combine(".", "elsewhere", "sample.csproj")
                );
            var testee = new Project(
                Path.Combine(".", "somewhere", "sample.csproj"),
                referencedProject
                );

            testee.WatchFolders.Should().BeEquivalentTo(new[] {
                Path.Combine(".", "elsewhere"),
                Path.Combine(".", "somewhere"),
            });
        }

        [Fact]
        public void DistinctWatchFolders()
        {
            var referencedProject = new Project(
                Path.Combine(".", "elsewhere", "sample.csproj")
                );
            var anotherProject = new Project(
                Path.Combine(".", "another", "sample.csproj"),
                referencedProject
                );
            var testee = new Project(
                Path.Combine(".", "somewhere", "sample.csproj"),
                referencedProject,
                anotherProject
                );

            testee.WatchFolders.Should().BeEquivalentTo(new[] {
                Path.Combine(".", "elsewhere"),
                Path.Combine(".", "somewhere"),
                Path.Combine(".", "another"),
            });
        }

        [Fact]
        public void GetsMatchProject()
        {
            var referencedProject = new Project(
                Path.Combine(".", "elsewhere", "sample.csproj")
                );
            var anotherProject = new Project(
                Path.Combine(".", "another", "sample.csproj"),
                referencedProject
                );
            var testee = new Project(
                Path.Combine(".", "somewhere", "sample.csproj"),
                anotherProject
                );

            var filePath = Path.Combine(".", "elsewhere", "foo.cs");

            testee.FindPertinentProject(filePath)
                .Should().Be(referencedProject);
        }
    }
}