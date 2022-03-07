using Microsoft.Extensions.DependencyInjection;

namespace Ponder;

public sealed class Bus : IBus
{
    public Bus(IServiceCollection services)
    {
        Provider = services.BuildServiceProvider();
    }

    public ServiceProvider Provider { get; }

    public void Publish<T>(T message)
    {
        var listeners = Provider.GetServices<IBusListener<T>>();

        foreach (var l in listeners)
        {
            l.Publish(message);
        }
    }
}
