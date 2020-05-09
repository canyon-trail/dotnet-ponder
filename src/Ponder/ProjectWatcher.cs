using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Ponder
{
    public sealed class ProjectWatcher
    {
        private readonly Project _project;
        private readonly IFilesystemWatcher _watcher;
        private readonly IScheduler _scheduler;

        public ProjectWatcher(Project project, IFilesystemWatcher watcher, IScheduler scheduler)
        {
            _project = project;
            _watcher = watcher;
            _scheduler = scheduler;
        }

        public IObservable<Project> GetChanges()
        {
            var fileChanges = _project.WatchFolders
                .Select(_watcher.WatchFolder)
                .Merge()
                .Where(x => _project.IsMatch(x))
                .Select(x => _project)
                .Throttle(TimeSpan.FromSeconds(0.25), _scheduler);

            return fileChanges.StartWith(_project).ObserveOn(_scheduler);
        }
    }
}
