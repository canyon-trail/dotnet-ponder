namespace Ponder.Old;

public interface IBus
{
    Task Publish<T>(T message) where T : notnull;
}
