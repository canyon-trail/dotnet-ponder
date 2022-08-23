using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ponder.Old.Exits;
using Ponder.Old.Sln;
using Ponder.Old.Tests.Sln;
using Xunit;

namespace Ponder.Old.Tests;

public sealed class SlnFinderTests : BusFixture
{
    [Fact]
    public async Task FindsSln()
    {
        var workingDir = "C:\\path\\to\\dir";
        var slnPath = Path.Combine(workingDir, "my-thing.sln");
        Filesystem.SetupGet(x => x.CurrentDirectory)
            .Returns(workingDir);
        Filesystem.Setup(x => x.ListFiles(workingDir, "*.sln"))
            .ReturnsAsync(ImmutableArray.Create(slnPath));

        var testee = Bus.Provider.GetRequiredService<SlnFinder>();

        await testee.FindSln();

        Messages.Last().Should().BeEquivalentTo(new SlnSelected(slnPath));
    }

    [Fact]
    public async Task FindsNoSln()
    {
        var workingDir = "C:\\path\\to\\dir";
        Filesystem.SetupGet(x => x.CurrentDirectory)
            .Returns(workingDir);
        Filesystem.Setup(x => x.ListFiles(workingDir, "*.sln"))
            .ReturnsAsync(ImmutableArray<string>.Empty);

        var testee = Bus.Provider.GetRequiredService<SlnFinder>();

        await testee.FindSln();

        Messages.Last().Should().BeEquivalentTo(
            new ErrorMessageAndExitSignal(
               "Unable to find sln file;" +
               " run in a directory with a single sln file " +
               "or specify the path to the sln"));
    }

    [Fact]
    public async Task FindsMultipleSlns()
    {
        var workingDir = "C:\\path\\to\\dir";
        Filesystem.SetupGet(x => x.CurrentDirectory)
            .Returns(workingDir);
        Filesystem.Setup(x => x.ListFiles(workingDir, "*.sln"))
            .ReturnsAsync(ImmutableArray.Create(
                Path.Combine(workingDir, "my-thing.sln"),
                Path.Combine(workingDir, "other-thing.sln")
                ));

        var testee = Bus.Provider.GetRequiredService<SlnFinder>();

        await testee.FindSln();

        Messages.Last().Should().BeEquivalentTo(
            new ErrorMessageAndExitSignal(
               "Found multiple sln files;" +
               " run in a directory with a single sln file " +
               "or specify the path to the sln"));
    }
}
