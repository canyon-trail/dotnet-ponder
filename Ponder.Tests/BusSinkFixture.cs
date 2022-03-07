using System.Collections.Generic;

namespace Ponder.Tests;

public sealed class BusSinkFixture : IBus
{
    private readonly List<object> _messages = new ();
    public IEnumerable<object> Messages => _messages;

    public void Publish<T>(T message)
    {
        _messages.Add(message);
    }
}