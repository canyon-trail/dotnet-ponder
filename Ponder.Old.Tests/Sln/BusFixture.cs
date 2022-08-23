using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Ponder.Old.Tests.Sln;

public class BusFixture
{
    private readonly List<object> _messages = new ();
    public Bus Bus { get; }
    
    public Mock<IFilesystem> Filesystem { get; }

    public IEnumerable<object> Messages => _messages;

    public BusFixture()
    {
        Filesystem = new Mock<IFilesystem>();
        Bus = new Bus(new ServiceCollection()
            .AddPonderServices()
            .ReplaceFilesystem(Filesystem.Object));

        Bus.OnPublish += _messages.Add;
    }
}