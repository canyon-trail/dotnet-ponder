using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Ponder.Exits;
using Xunit;

namespace Ponder.Tests;

public sealed class SlnFinderTests
{
    [Fact]
    public async Task FindsSln()
    {
        var filesystem = new Mock<IFilesystem>();
        var bus = new BusSinkFixture();
        var workingDir = "C:\\path\\to\\dir";
        var slnPath = Path.Combine(workingDir, "my-thing.sln");
        filesystem.SetupGet(x => x.CurrentDirectory)
            .Returns(workingDir);
        filesystem.Setup(x => x.ListFiles(workingDir, "*.sln"))
            .ReturnsAsync(ImmutableArray.Create(slnPath));

        var testee = new SlnFinder(
            filesystem.Object,
            bus
            );

        await testee.FindSln();

        bus.Messages.Should().BeEquivalentTo(new SlnSelected(slnPath));
    }

    [Fact]
    public async Task FindsNoSln()
    {
        var filesystem = new Mock<IFilesystem>();
        var bus = new BusSinkFixture();
        var workingDir = "C:\\path\\to\\dir";
        filesystem.SetupGet(x => x.CurrentDirectory)
            .Returns(workingDir);
        filesystem.Setup(x => x.ListFiles(workingDir, "*.sln"))
            .ReturnsAsync(ImmutableArray<string>.Empty);

        var testee = new SlnFinder(
            filesystem.Object,
            bus
            );

        await testee.FindSln();

        bus.Messages.Should().BeEquivalentTo(
            new ErrorMessageAndExitSignal(
               "Unable to find sln file;" +
               " run in a directory with a single sln file " +
               "or specify the path to the sln"));
    }

    [Fact]
    public async Task FindsMultipleSlns()
    {
        var filesystem = new Mock<IFilesystem>();
        var bus = new BusSinkFixture();
        var workingDir = "C:\\path\\to\\dir";
        filesystem.SetupGet(x => x.CurrentDirectory)
            .Returns(workingDir);
        filesystem.Setup(x => x.ListFiles(workingDir, "*.sln"))
            .ReturnsAsync(ImmutableArray.Create(
                Path.Combine(workingDir, "my-thing.sln"),
                Path.Combine(workingDir, "other-thing.sln")
                ));

        var testee = new SlnFinder(
            filesystem.Object,
            bus
            );

        await testee.FindSln();

        bus.Messages.Should().BeEquivalentTo(
            new ErrorMessageAndExitSignal(
               "Found multiple sln files;" +
               " run in a directory with a single sln file " +
               "or specify the path to the sln"));
    }
}
