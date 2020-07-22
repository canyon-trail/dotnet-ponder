using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using Moq;
using Xunit;

namespace Ponder.Tests
{
    public sealed class ProjectWatcherTests
    {
        private readonly Project _project;
        private readonly Mock<IFilesystemWatcher> _watcher;
        private readonly ProjectWatcher _testee;
        private readonly TestScheduler _scheduler;

        public ProjectWatcherTests()
        {
            _project = new Project(RelPath.FromString("Example.csproj"));
            _watcher = new Mock<IFilesystemWatcher>(MockBehavior.Strict);
            _scheduler = new TestScheduler();
            _testee = new ProjectWatcher(_project, _watcher.Object, _scheduler, new Mock<ILogger<ProjectWatcher>>().Object);
        }

        [Fact]
        public void PublishesImmediately()
        {
            _watcher
                .Setup(x => x.WatchFolder(_project.WatchFolders.Single()))
                .Returns(Observable.Never(RelPath.Empty));

            var messages = new List<Project>();
            _testee.GetChanges().Subscribe(messages.Add);
            _scheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);
            messages
                .Should()
                .BeEquivalentTo(new[] { _project });
        }

        [Fact]
        public void PublishesOnFileChange()
        {
            _watcher
                .Setup(x => x.WatchFolder(_project.WatchFolders.Single()))
                .Returns(new[] {
                    ValidCsFile
                }.ToObservable());

            var messages = new List<Project>();
            _testee.GetChanges().Subscribe(messages.Add);
            _scheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);
            messages
                .Should()
                .BeEquivalentTo(new[] { _project, _project });
        }

        [Fact]
        public void NoPublishOnIrrelevantFile()
        {
            _watcher
                .Setup(x => x.WatchFolder(_project.WatchFolders.Single()))
                .Returns(new[] {
                    _project.ProjectFolder.Append("File1.txt")
                }.ToObservable());

            var messages = new List<Project>();
            _testee.GetChanges().Subscribe(messages.Add);
            _scheduler.AdvanceTo(TimeSpan.FromSeconds(1).Ticks);
            messages
                .Should()
                .BeEquivalentTo(new[] { _project });
        }

        [Fact]
        public void DebouncesAtQuarterSecond()
        {
            var subject = new Subject<RelPath>();
            _watcher
                .Setup(x => x.WatchFolder(_project.WatchFolders.Single()))
                .Returns(subject.ObserveOn(_scheduler));

            new List<double> { 1, 1.1, 1.2, 1.45 }
                .ForEach(x => _scheduler.Schedule(Unit.Default, TimeSpan.FromSeconds(x), (_, __) => {
                    subject.OnNext(ValidCsFile);
                    return Disposable.Empty;
                }));

            var count = 0;
            _testee.GetChanges().Subscribe(x => count++);

            _scheduler.AdvanceTo(TimeSpan.FromSeconds(1.44).Ticks);
            count.Should().Be(1);
            _scheduler.AdvanceTo(TimeSpan.FromSeconds(1.451).Ticks);
            count.Should().Be(2);
            _scheduler.AdvanceTo(TimeSpan.FromSeconds(1.701).Ticks);
            count.Should().Be(3);
        }

        private RelPath ValidCsFile => _project.ProjectFolder.Append("File1.cs");
    }
}