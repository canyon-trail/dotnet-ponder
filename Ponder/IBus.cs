namespace Ponder;

public interface IBus
{
    Task Publish<T>(T message) where T : notnull;
}
