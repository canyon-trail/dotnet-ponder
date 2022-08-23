using Ponder.Old.Exits;
using Ponder.Old.Sln;

namespace Ponder.Old;

public sealed class SlnFinder
{
    private readonly IFilesystem _filesystem;
    private readonly IBus _bus;

    public SlnFinder(IFilesystem filesystem, IBus bus)
    {
        _filesystem = filesystem;
        _bus = bus;
    }

    public async Task FindSln()
    {
        var files = await _filesystem.ListFiles(_filesystem.CurrentDirectory, "*.sln");

        if (files.Length == 0)
        {
            await _bus.Publish(new ErrorMessageAndExitSignal(
                "Unable to find sln file;" +
                " run in a directory with a single sln file " +
                "or specify the path to the sln"));
        }
        else if (files.Length > 1)
        {
            await _bus.Publish(new ErrorMessageAndExitSignal(
                "Found multiple sln files;" +
                " run in a directory with a single sln file " +
                "or specify the path to the sln"));
        }
        else
        {
            await _bus.Publish(new SlnSelected(files[0]));
        }
    }
}
