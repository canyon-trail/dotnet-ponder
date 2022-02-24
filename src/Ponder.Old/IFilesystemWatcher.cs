using System;

namespace Ponder.Old
{
    public interface IFilesystemWatcher
    {
        IObservable<RelPath> WatchFolder(RelPath folder);
    }
}