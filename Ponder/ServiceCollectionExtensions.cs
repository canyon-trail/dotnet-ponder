using Microsoft.Extensions.DependencyInjection;
using Ponder.Exits;

namespace Ponder;

// because this is apparently the way we do it...
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPonderServices(this IServiceCollection services)
    {
        // TODO: write a test to ensure that any IBusListener<T> implementer is registered;
        //  but don't do assembly scanning (for startup performance reasons)
        services.AddSingleton<ExitGate>();
        services.AddSingleton<IBusListener<ExitSignal>>(ctx => ctx.GetRequiredService<ExitGate>());
        services.AddTransient<IBusListener<ErrorMessageAndExitSignal>, ErrorExit>();
        services.AddTransient<SlnFinder>();
        services.AddTransient<IBusListener<SlnSelected>, SlnLoader>();
        services.AddTransient<IFilesystem, RealFilesystem>();

        return services;
    }
}
