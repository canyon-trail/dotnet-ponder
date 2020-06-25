using System;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using io = System.IO;

namespace Ponder
{
    public sealed class FilesystemWatcher : IFilesystemWatcher
    {
        private readonly ILogger<FilesystemWatcher> _logger;

        public FilesystemWatcher(ILogger<FilesystemWatcher> logger)
        {
            _logger = logger;
        }

        public IObservable<string> WatchFolder(string folder)
        {
            var baseObservable = Observable.Create<string>(observer => {
                var absPath = io.Path.GetFullPath(folder);
                var watcher = new io.FileSystemWatcher(absPath)
                {
                    IncludeSubdirectories = true
                };

                watcher.Changed += (o, e) => observer.OnNext(io.Path.GetRelativePath(folder, e.FullPath));
                watcher.Created += (o, e) => observer.OnNext(io.Path.GetRelativePath(folder, e.FullPath));
                watcher.Deleted += (o, e) => observer.OnNext(io.Path.GetRelativePath(folder, e.FullPath));
                watcher.Renamed += (o, e) => observer.OnNext(io.Path.GetRelativePath(folder, e.FullPath));

                return watcher;
            });

            return baseObservable
                .Select(x => {
                    _logger.LogWarning("Got change for path {changedPath}", x);

                    return x;
                });
        }
    }
}
