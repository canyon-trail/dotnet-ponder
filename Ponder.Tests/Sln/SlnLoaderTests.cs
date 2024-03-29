﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FSharp.Collections;
using Moq;
using Ponder.Parsers;
using Ponder.Sln;
using Xunit;

namespace Ponder.Tests.Sln;

public sealed class SlnLoaderTests
{
    [Fact]
    public async Task HappyPathFromSlnSelected()
    {
        var filesystem = new Mock<IFilesystem>();
        var bus = new Bus(new ServiceCollection()
            .AddPonderServices()
            .ReplaceFilesystem(filesystem.Object));

        var slnPath = "C:\\proj\\myproj.sln";
        var sln = new SlnParser.SlnFile(
            FSharpList<SlnParser.SlnProject>.Cons(
                new SlnParser.SlnProject("aproj", "C:\\proj\\aproj\\aproj.csproj"),
                FSharpList<SlnParser.SlnProject>.Empty
            ));
        filesystem.Setup(x => x.LoadSln(slnPath))
            .ReturnsAsync(sln);
        filesystem.Setup(x => x.Exists(slnPath)).Returns(true);

        var published = new List<object>();
        bus.OnPublish += published.Add;

        await bus.Publish(new SlnSelected(slnPath));

        var expected = new SlnLoaded(
            ImmutableArray.Create(sln.Projects.Head)
        );
        var actual = published
            .OfType<SlnLoaded>()
            .SingleOrDefault();
        actual.Should().NotBeNull();
        actual.Projects.Should().BeEquivalentTo(expected.Projects);
    }
}
