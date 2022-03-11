using Microsoft.Extensions.DependencyInjection;

namespace Ponder;

public sealed class Bus : IBus
{
    public Bus(IServiceCollection services)
    {
        services.AddSingleton<IBus>(this);
        Provider = services.BuildServiceProvider();
    }

    public ServiceProvider Provider { get; }

    public async Task Publish<T>(T message) where T : notnull
    {
        var listeners = Provider.GetServices<IBusListener<T>>();

        foreach (var l in listeners)
        {
            await l.OnPublish(message);
        }

        _onPublish.Invoke(message);
    }

    private Action<object> _onPublish = x => { };

    public event Action<object> OnPublish
    {
        add
        {
            lock (this)
            {
                _onPublish += value;
            }
        }
        remove
        {
            lock (this)
            {
                _onPublish -= value;
            }
        }
    }
}
