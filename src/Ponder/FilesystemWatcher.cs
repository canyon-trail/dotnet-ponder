using System;
using System.Reactive.Linq;
using io = System.IO;

namespace Ponder
{
    public sealed class FilesystemWatcher : IFilesystemWatcher
    {
        public IObservable<string> WatchFolder(string folder)
        {
            return Observable.Create<string>(observer => {
                var watcher = new io.FileSystemWatcher(folder)
                {
                    IncludeSubdirectories = true
                };

                watcher.Changed += (o, e) => observer.OnNext(io.Path.GetRelativePath(folder, e.FullPath));
                watcher.Created += (o, e) => observer.OnNext(io.Path.GetRelativePath(folder, e.FullPath));
                watcher.Deleted += (o, e) => observer.OnNext(io.Path.GetRelativePath(folder, e.FullPath));
                watcher.Renamed += (o, e) => observer.OnNext(io.Path.GetRelativePath(folder, e.FullPath));

                return watcher;
            });
        }
    }
}
