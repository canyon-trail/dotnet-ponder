using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ponder.Tests;

public sealed class BusSinkFixture : IBus
{
    private readonly List<object> _messages = new ();
    public IEnumerable<object> Messages => _messages;

    public Task Publish<T>(T message) where T : notnull
    {
        _messages.Add(message);

        return Task.CompletedTask;
    }
}
