using FluentAssertions;
using Ponder.Old;
using Xunit;

namespace Ponder.Tests.Old
{
    public sealed class ProjectTests
    {
        [Fact]
        public void DefaultFilePaths()
        {
            var testee = new Project(
                RelPath.Empty.Append(".", "somewhere", "sample.csproj")
                );

            var expected = new[] {
                testee.CsProjPath,
                RelPath.Empty.Append(".", "somewhere", "**", "*.cs")
            };

            testee.WatchPaths.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ProjectFolder()
        {
            var testee = new Project(
                RelPath.Empty.Append(".", "somewhere", "sample.csproj")
                );

            var expected = RelPath.Empty.Append(".", "somewhere");

            testee.ProjectFolder.Should().Be(expected);
        }

        [Theory]
        [InlineData(true, "somewhere", "foo", "bar.cs")]
        [InlineData(false, "somewhere", "foo", "bar.png")]
        [InlineData(false, "elsewhere", "foo", "bar.cs")]
        [InlineData(true, "somewhere", "sample.csproj")]
        public void FileMatches(bool isMatch, params string[] segments)
        {
            var fullPath = RelPath.Empty.Append(segments);

            var testee = new Project(
                RelPath.Empty.Append(".", "somewhere", "sample.csproj")
                );

            testee.IsMatch(fullPath).Should().Be(isMatch);
        }

        [Fact]
        public void TransitiveWatchFolders()
        {
            var referencedProject = new Project(
                RelPath.Empty.Append(".", "elsewhere", "sample.csproj")
                );
            var testee = new Project(
                RelPath.Empty.Append(".", "somewhere", "sample.csproj"),
                referencedProject
                );

            testee.WatchFolders.Should().BeEquivalentTo(new[] {
                RelPath.Empty.Append(".", "elsewhere"),
                RelPath.Empty.Append(".", "somewhere"),
            });
        }

        [Theory]
        [InlineData(true, "elsewhere", "foo", "bar.cs")]
        public void TransitiveMatches(bool expected, params string[] segments)
        {
            var referencedProject = new Project(
                RelPath.Empty.Append(".", "elsewhere", "sample.csproj")
                );
            var testee = new Project(
                RelPath.Empty.Append(".", "somewhere", "sample.csproj"),
                referencedProject
                );


            var path = RelPath.Empty.Append(segments);

            testee.IsMatch(path).Should().Be(expected);
        }

        [Fact]
        public void DistinctWatchFolders()
        {
            var referencedProject = new Project(
                RelPath.Empty.Append(".", "elsewhere", "sample.csproj")
                );
            var anotherProject = new Project(
                RelPath.Empty.Append(".", "another", "sample.csproj"),
                referencedProject
                );
            var testee = new Project(
                RelPath.Empty.Append(".", "somewhere", "sample.csproj"),
                referencedProject,
                anotherProject
                );

            testee.WatchFolders.Should().BeEquivalentTo(new[] {
                RelPath.Empty.Append(".", "elsewhere"),
                RelPath.Empty.Append(".", "somewhere"),
                RelPath.Empty.Append(".", "another"),
            });
        }

        [Fact]
        public void GetsMatchProject()
        {
            var referencedProject = new Project(
                RelPath.Empty.Append(".", "elsewhere", "sample.csproj")
                );
            var anotherProject = new Project(
                RelPath.Empty.Append(".", "another", "sample.csproj"),
                referencedProject
                );
            var testee = new Project(
                RelPath.Empty.Append(".", "somewhere", "sample.csproj"),
                anotherProject
                );

            var filePath = RelPath.Empty.Append(".", "elsewhere", "foo.cs");

            testee.FindPertinentProject(filePath)
                .Should().Be(referencedProject);
        }
    }
}
