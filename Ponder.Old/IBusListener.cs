namespace Ponder.Old;

public interface IBusListener<in T>
{
    Task OnPublish(T message);
}
