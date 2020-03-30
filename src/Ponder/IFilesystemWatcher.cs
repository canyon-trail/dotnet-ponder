using System;

namespace Ponder
{
    public interface IFilesystemWatcher
    {
        IObservable<string> WatchFolder(string folder);
    }
}