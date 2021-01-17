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

        public IObservable<RelPath> WatchFolder(RelPath folder)
        {
            var absPath = io.Path.GetFullPath(folder.Path);

            var relativeRoot = absPath;
            foreach(var segment in folder.Segments)
            {
                relativeRoot = io.Directory.GetParent(relativeRoot).FullName;
            }

            _logger.LogDebug("Starting filesystem watcher at {path}; relative path: {folder}; relative root: {relativeRoot}", absPath, folder, relativeRoot);
            var baseObservable = Observable.Create<string>(observer => {
                    var watcher = new io.FileSystemWatcher(absPath)
                    {
                        IncludeSubdirectories = true
                    };

                    watcher.Changed += (o, e) => observer.OnNext(e.FullPath);
                    watcher.Created += (o, e) => observer.OnNext(e.FullPath);
                    watcher.Deleted += (o, e) => observer.OnNext(e.FullPath);
                    watcher.Renamed += (o, e) => observer.OnNext(e.FullPath);

                    watcher.EnableRaisingEvents = true;

                    return watcher;
                })
                .Select(x => io.Path.GetRelativePath(relativeRoot, x))
                .Select(RelPath.FromString);

            return baseObservable
                .Select(x => {
                    _logger.LogDebug("Got change for path {changedPath}", x);

                    return x;
                });
        }
    }
}
