using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Ponder.Old.Projects;
using Ponder.Old.Sln;
using Xunit;

namespace Ponder.Old.Tests.Sln;

public sealed class SlnStateReactorTests : BusFixture
{
    [Fact]
    public async Task PublishesInitialState()
    {
        await Bus.Publish(new SlnLoaded(ImmutableArray.Create(
            new SlnParser.SlnProject("proj1", "proj1/proj1.csproj"),
            new SlnParser.SlnProject("proj2", "proj2/proj2.csproj")
            )));

        Messages
            .OfType<SlnState>()
            .Should()
            .BeEquivalentTo(
                new[] {
                    new SlnState(
                        ImmutableArray.Create<ProjectState>(
                            new LoadingProjectState("proj1", "proj1/proj1.csproj"),
                            new LoadingProjectState("proj2", "proj2/proj2.csproj")
                        )
                    )
                },
                x => x.ComparingByMembers<SlnState>()
                );
    }

    [Fact]
    public async Task PublishesSingleProjectLoaded()
    {
        await Bus.Publish(new SlnLoaded(ImmutableArray.Create(
            new SlnParser.SlnProject("proj1", "proj1/proj1.csproj"),
            new SlnParser.SlnProject("proj2", "proj2/proj2.csproj")
            )));

        var loadedProject = new ProjectInfo(
            "proj1",
            "proj1/proj1.csproj",
            References: ImmutableArray<string>.Empty,
            IsTestProject: false
            );
        await Bus.Publish(new ProjectLoaded(loadedProject));

        Messages
            .OfType<SlnState>()
            .LastOrDefault()
            .Should()
            .BeEquivalentTo(
                new SlnState(
                    ImmutableArray.Create<ProjectState>(
                        new LoadedProjectState(loadedProject),
                        new LoadingProjectState("proj2", "proj2/proj2.csproj")
                    )
                ),
                x => x.ComparingByMembers<SlnState>()
                );
    }
}
