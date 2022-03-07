namespace Ponder;

public interface IBusListener<in T>
{
    void OnPublish(T message);
}
