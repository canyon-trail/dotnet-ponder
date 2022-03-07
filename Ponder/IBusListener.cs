namespace Ponder;

public interface IBusListener<in T>
{
    void Publish(T message);
}