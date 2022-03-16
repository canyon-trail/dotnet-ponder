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
    public async Task OnPublish(SlnLoaded message)
    {
        var parseProjectTasks = message.Projects
            .Select(x => _filesystem.LoadProject(x.Path));

        var projects = await Task.WhenAll(parseProjectTasks);

        await Task.WhenAll(
            projects.Select(x => _bus.Publish(new ProjectLoaded(x))));
    }
}
