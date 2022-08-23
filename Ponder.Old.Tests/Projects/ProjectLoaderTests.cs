using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ponder.Old.Projects;
using Ponder.Old.Sln;
using Xunit;

namespace Ponder.Old.Tests.Projects;

public sealed class ProjectLoaderTests
{
    [Fact]
    public async Task HappyPathFromSlnLoaded()
    {
        var filesystem = new Mock<IFilesystem>();
        var bus = new Bus(new ServiceCollection().AddPonderServices().ReplaceFilesystem(filesystem.Object));

        var projectPath = "C:\\Src\\Example\\Example.csproj";
        var projectInfo = new ProjectInfo("Example", projectPath, ImmutableArray<string>.Empty, false);
        filesystem.Setup(x => x.LoadProject(projectPath))
            .ReturnsAsync(projectInfo);

        var published = new List<object>();

        bus.OnPublish += published.Add;

        await bus.Publish(new SlnLoaded(ImmutableArray.Create(new SlnParser.SlnProject("Example", projectPath))));

        published.Should().Contain(new ProjectLoaded(projectInfo));
    }
}
