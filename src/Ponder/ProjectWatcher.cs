using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;

namespace Ponder
{
    public sealed class ProjectWatcher
    {
        private readonly Project _project;
        private readonly IFilesystemWatcher _watcher;
        private readonly IScheduler _scheduler;
        private readonly ILogger<ProjectWatcher> _logger;

        public ProjectWatcher(Project project, IFilesystemWatcher watcher, IScheduler scheduler, ILogger<ProjectWatcher> logger)
        {
            _project = project;
            _watcher = watcher;
            _scheduler = scheduler;
            _logger = logger;
        }

        public IObservable<Project> GetChanges()
        {
            var fileChanges = _project.WatchFolders
                .Select(x => {
                    _logger.LogWarning("Watching folder {folder}", x);

                    return x;
                })
                .Select(_watcher.WatchFolder)
                .Merge()
                .Where(x => _project.IsMatch(x))
                .Select(x => _project)
                .Throttle(TimeSpan.FromSeconds(0.25), _scheduler);

            return fileChanges.StartWith(_project).ObserveOn(_scheduler);
        }
    }
}
