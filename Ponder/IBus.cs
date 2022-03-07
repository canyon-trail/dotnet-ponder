namespace Ponder;

public interface IBus
{
    void Publish<T>(T message);
}