using Ponder.Sln;

namespace Ponder.Projects;

public sealed class ProjectLoader : IBusListener<SlnLoaded>
{
    private readonly IBus _bus;
    private readonly IFilesystem _filesystem;

    public ProjectLoader(IBus bus, IFilesystem filesystem)
    {
        _bus = bus;
        _filesystem = filesystem;
    }
    public Task OnPublish(SlnLoaded message)
    {
        Task.Run(async () =>
        {
            await Task.WhenAll(
                message.Projects
                    .Select(x => _filesystem.LoadProject(x.Path))
                    .Select(x => x.ContinueWith(p => _bus.Publish(new ProjectLoaded(p.Result))))
            );
        });

        return Task.CompletedTask;
    }
}
