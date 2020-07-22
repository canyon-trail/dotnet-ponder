using System;

namespace Ponder
{
    public interface IFilesystemWatcher
    {
        IObservable<RelPath> WatchFolder(RelPath folder);
    }
}