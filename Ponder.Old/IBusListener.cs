namespace Ponder;

public interface IBusListener<in T>
{
    Task OnPublish(T message);
}
