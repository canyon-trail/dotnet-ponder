using Ponder.Exits;

namespace Ponder;

public sealed class SlnLoader : IBusListener<SlnSelected>
{
    private readonly IBus _bus;

    public SlnLoader(IBus bus)
    {
        _bus = bus;
    }

    public void OnPublish(SlnSelected message)
    {
        LoadSln(message.SlnPath).GetAwaiter().GetResult();
    }

    private async Task LoadSln(string slnPath)
    {
        if (!File.Exists(slnPath))
        {
            Console.WriteLine($"File {slnPath} not found;" +
            " run in a directory with a single sln file " +
            "or specify the path to the sln");

            _bus.Publish(new ExitSignal());
            return;
        }

        await using var slnStream = File.OpenRead(slnPath);

        var slnFile = await Ponder.Parsers.SlnParser.parseSlnFromStream(slnStream);

        Console.WriteLine("Projects:");
        foreach (var project in slnFile.Projects)
        {
            Console.WriteLine($"\t{project.Name}: {project.Path}");
        }

        _bus.Publish(new ExitSignal());
    }
}
