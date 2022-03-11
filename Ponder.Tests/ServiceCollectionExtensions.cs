using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Ponder.Tests;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ReplaceFilesystem(this IServiceCollection services, IFilesystem implementation)
    {
        services.RemoveAll(typeof(IFilesystem));

        services.AddSingleton(implementation);

        return services;
    }
}
