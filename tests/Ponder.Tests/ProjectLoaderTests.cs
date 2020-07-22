using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Ponder.Tests
{
    public sealed class ProjectLoaderTests
    {
        private readonly Mock<IFilesystem> _filesystem;
        private readonly ProjectLoader _testee;

        public ProjectLoaderTests()
        {
            _filesystem = new Mock<IFilesystem>(MockBehavior.Strict);
            _testee = new ProjectLoader(_filesystem.Object, new Mock<ILogger<ProjectLoader>>().Object);
        }

        [Fact]
        public void LoadsIslandProject()
        {
            var projectFilePath = RelPath.FromString("./Example.csproj");
            SetupLeafProject(projectFilePath);
            var project = _testee.Load(projectFilePath);

            project.Should().BeEquivalentTo(new Project(projectFilePath));
        }

        [Fact]
        public void LoadsSingleTransitiveProject()
        {
            var rootProjectText = @"<Project Sdk=""Microsoft.NET.Sdk"">
                <ItemGroup>
                    <ProjectReference Include=""..\..\other\other.csproj"" />
                </ItemGroup>
            </Project>";

            var projectFilePath = RelPath.Empty.Append(".", "example", "Example.csproj");
            var otherProjectPath = RelPath.Empty.Append("..", "other", "other.csproj");
            _filesystem
                .Setup(x => x.ReadFile(projectFilePath.Path))
                .Returns(rootProjectText);
            SetupLeafProject(otherProjectPath);
            var project = _testee.Load(projectFilePath);

            project.Should().BeEquivalentTo(new Project(projectFilePath, new Project(otherProjectPath)));
        }

        [Fact]
        public void LoadsTwoLevelsOfNesting()
        {
            var rootProjectText = @"<Project Sdk=""Microsoft.NET.Sdk"">
                <ItemGroup>
                    <ProjectReference Include=""..\..\other\other.csproj"" />
                </ItemGroup>
            </Project>";
            var otherProjectText = @"<Project Sdk=""Microsoft.NET.Sdk"">
                <ItemGroup>
                    <ProjectReference Include=""..\yetAnother\yetAnother.csproj"" />
                </ItemGroup>
            </Project>";

            var projectFilePath = RelPath.Empty.Append(".", "example", "Example.csproj");
            var otherProjectPath = RelPath.Empty.Append("..", "other", "other.csproj");
            var yetAnotherProjectPath = RelPath.Empty.Append("..", "yetAnother", "yetAnother.csproj");
            _filesystem
                .Setup(x => x.ReadFile(projectFilePath.Path))
                .Returns(rootProjectText);
            _filesystem
                .Setup(x => x.ReadFile(otherProjectPath.Path))
                .Returns(otherProjectText);
            SetupLeafProject(yetAnotherProjectPath);
            var project = _testee.Load(projectFilePath);

            project.Should()
                .BeEquivalentTo(
                    new Project(
                        projectFilePath,
                        new Project(
                            otherProjectPath,
                            new Project(yetAnotherProjectPath)))
                    );
        }

        // TODO: no reload same project twice?

        private void SetupLeafProject(RelPath projectPath)
        {
            var projectText = @"<Project Sdk=""Microsoft.NET.Sdk""></Project>";
            _filesystem
                .Setup(x => x.ReadFile(projectPath.Path))
                .Returns(projectText);
        }
    }
}