using System.IO;
using FluentAssertions;
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
            _testee = new ProjectLoader(_filesystem.Object);
        }

        [Fact]
        public void LoadsIslandProject()
        {
            string projectFilePath = "./Example.csproj";
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

            string projectFilePath = Path.Combine(".", "example", "Example.csproj");
            string otherProjectPath = Path.Combine("..", "other", "other.csproj");
            _filesystem
                .Setup(x => x.ReadFile(projectFilePath))
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

            string projectFilePath = Path.Combine(".", "example", "Example.csproj");
            string otherProjectPath = Path.Combine("..", "other", "other.csproj");
            string yetAnotherProjectPath = Path.Combine("..", "yetAnother", "yetAnother.csproj");
            _filesystem
                .Setup(x => x.ReadFile(projectFilePath))
                .Returns(rootProjectText);
            _filesystem
                .Setup(x => x.ReadFile(otherProjectPath))
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

        private void SetupLeafProject(string projectPath)
        {
            var projectText = @"<Project Sdk=""Microsoft.NET.Sdk""></Project>";
            _filesystem
                .Setup(x => x.ReadFile(projectPath))
                .Returns(projectText);
        }
    }
}