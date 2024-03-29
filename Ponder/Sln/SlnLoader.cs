﻿using System.Collections.Immutable;
using Ponder.Exits;

namespace Ponder.Sln;

public sealed class SlnLoader : IBusListener<SlnSelected>
{
    private readonly IBus _bus;
    private readonly IFilesystem _filesystem;

    public SlnLoader(IBus bus, IFilesystem filesystem)
    {
        _bus = bus;
        _filesystem = filesystem;
    }

    public async Task OnPublish(SlnSelected message)
    {
        await LoadSln(message.SlnPath);
    }

    private async Task LoadSln(string slnPath)
    {
        if (!_filesystem.Exists(slnPath))
        {
            Console.WriteLine($"File {slnPath} not found;" +
            " run in a directory with a single sln file " +
            "or specify the path to the sln");

            await _bus.Publish(new ExitSignal());
            return;
        }

        var slnRoot = Path.GetDirectoryName(slnPath);
        Directory.SetCurrentDirectory(slnRoot!);
        slnPath = Path.GetFileName(slnPath);

        var slnFile = await _filesystem.LoadSln(slnPath);

        await _bus.Publish(new SlnLoaded(ImmutableArray.CreateRange(slnFile.Projects)));
    }
}
