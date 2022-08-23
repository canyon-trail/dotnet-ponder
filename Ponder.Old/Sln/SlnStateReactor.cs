using System.Collections.Immutable;
using Ponder.Old.Projects;

namespace Ponder.Old.Sln;

public sealed class SlnStateReactor
    : IBusListener<SlnLoaded>
    , IBusListener<ProjectLoaded>
{
    private readonly IBus _bus;

    public SlnStateReactor(IBus bus)
    {
        _bus = bus;
    }

    private SlnState? _state;
    public async Task OnPublish(SlnLoaded message)
    {
        Console.WriteLine($"handling SlnLoaded {message}");
        var state = _state;
        lock (this)
        {
            state = _state = new SlnState(
                message.Projects
                    .Select(x => new LoadingProjectState(x.Name, x.Path))
                    .Cast<ProjectState>()
                    .ToImmutableArray()
            );
        }
        await _bus.Publish(state);
    }

    public async Task OnPublish(ProjectLoaded message)
    {
        var state = _state;
        lock (this)
        {
            state = _state;
            if (state == null)
                return;
            var project = state.Projects.Single(x => x.Name == message.Project.Name);
            state = _state = new SlnState(
                state.Projects
                    .Select(x => x == project ? new LoadedProjectState(message.Project) : x)
                    .ToImmutableArray()
            );
        }
        await _bus.Publish(state);
    }
}
