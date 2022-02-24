using FluentAssertions;
using Moq;
using Ponder.Old;
using Xunit;

namespace Ponder.Tests
{
    public sealed class ProjectFinderTests
    {
        private readonly Mock<IFilesystem> _filesystem;
        private readonly ProjectFinder _testee;

        public ProjectFinderTests()
        {
            _filesystem = new Mock<IFilesystem>(MockBehavior.Strict);
            _testee = new ProjectFinder(_filesystem.Object);
        }

        [Fact]
        public void FindsProject()
        {
            _filesystem
                .Setup(x => x.GetCsProjFiles("."))
                .Returns(new[] { "./Found.csproj" });

            var result = _testee.FindProject(".");

            result.Should().BeEquivalentTo(new ProjectFindResult
            {
                ProjectPath = "./Found.csproj",
                IsFound = true,
                Candidates = new[] { "./Found.csproj" }
            });
        }

        [Fact]
        public void FileNotFound()
        {
            _filesystem
                .Setup(x => x.FileExists("./Found.csproj"))
                .Returns(false);

            var result = _testee.FindProject("./Found.csproj");

            result.Should().BeEquivalentTo(new ProjectFindResult
            {
                ProjectPath = "./Found.csproj",
                IsFound = false,
                Candidates = new[] { "./Found.csproj" }
            });
        }

        [Fact]
        public void FindsProjectByFullPath()
        {
            _filesystem
                .Setup(x => x.FileExists("./Found.csproj"))
                .Returns(true);

            var result = _testee.FindProject("./Found.csproj");

            result.Should().BeEquivalentTo(new ProjectFindResult
            {
                ProjectPath = "./Found.csproj",
                IsFound = true,
                Candidates = new[] { "./Found.csproj" }
            });
        }

        [Fact]
        public void NoProjectInDir()
        {
            _filesystem.Setup(x => x.GetCsProjFiles(".")).Returns(new string[0]);

            var result = _testee.FindProject(".");

            result.Should().BeEquivalentTo(new ProjectFindResult
            {
                IsFound = false,
                Candidates = new string[0]
            });
        }

        [Fact]
        public void MultipleProjects()
        {
            var projects = new[] {
                "./Derp.csproj",
                "./Wat.csproj",
            };
            _filesystem.Setup(x => x.GetCsProjFiles(".")).Returns(projects);

            var result = _testee.FindProject(".");

            result.Should().BeEquivalentTo(new ProjectFindResult
            {
                IsFound = false,
                Candidates = projects
            });
        }
    }
}
