using Microsoft.Extensions.DependencyInjection;

namespace Ponder;

// because this is apparently the way we do it...
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPonderServices(this IServiceCollection services)
    {
        services.AddSingleton<ExitGate>();
        services.AddSingleton<IBusListener<ExitSignal>>(ctx => ctx.GetRequiredService<ExitGate>());

        return services;
    }
}
