using Microsoft.Extensions.DependencyInjection;
using Ponder.Old.Display;
using Ponder.Old.Exits;
using Ponder.Old.Projects;
using Ponder.Old.Sln;

namespace Ponder.Old;

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

        services.AddTransient<ProjectLoader>();
        services.AddTransient<IBusListener<SlnLoaded>, ProjectLoader>();

        services.AddTransient<IBusListener<SlnSelected>, SlnLoader>();
        services.AddTransient<IFilesystem, RealFilesystem>();

        services.AddSingleton<SlnStateReactor>();
        services.AddTransient<IBusListener<SlnLoaded>>(x => x.GetRequiredService<SlnStateReactor>());
        services.AddTransient<IBusListener<ProjectLoaded>>(x => x.GetRequiredService<SlnStateReactor>());

        services.AddSingleton<SlnDisplay>();
        services.AddTransient<IBusListener<SlnState>>(x => x.GetRequiredService<SlnDisplay>());

        return services;
    }
}
