using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Ponder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.CompletedTask;

            var projectPath = args.FirstOrDefault() ?? ".";

            var finder = new ProjectFinder(new RealFilesystem());

            var findResult = finder.FindProject(projectPath);

            if(!findResult.IsFound)
            {
                Console.WriteLine($"Unable to locate csproj file; candidates: {string.Join(", ", findResult.Candidates)}");
                return;
            }

            var project = new Project(findResult.ProjectPath);

            var watcher = new ProjectWatcher(project, new FilesystemWatcher(), Scheduler.Default);

            var testRunFactory = new TestRunFactory();

            var outputStream = watcher.GetChanges()
                .Select(x => testRunFactory.RunTests(x))
                .StartWith(testRunFactory.RunTests(project))
                .Switch();

            var subscription = outputStream.Subscribe(str => Console.WriteLine(str));

            var mre = new System.Threading.ManualResetEvent(false);

            Console.CancelKeyPress += (o,e) => mre.Set();

            mre.WaitOne();
        }
    }

    public sealed class TestRunFactory : ITestRunFactory
    {
        public IObservable<string> RunTests(Project project)
        {
            return Observable.Create<string>(observer => {
                var psi = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"test \"{project.CsProjPath}\"",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true
                };

                var process = Process.Start(psi);

                var subscription = Observable.FromAsync(process.StandardOutput.ReadLineAsync)
                            .Repeat()
                            .TakeWhile(line => line != null)
                            .Subscribe(observer);

                return Disposable.Create(() => {
                    subscription.Dispose();
                    process.StandardInput.Write((char)3);
                });
            });
        }
    }
}
