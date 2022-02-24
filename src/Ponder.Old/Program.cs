using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ponder.Old
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var serviceProvider = new ServiceCollection()
                .AddLogging(b => b.AddConfiguration(config.GetSection("Logging")).AddDebug())
                .AddTransient<ProjectLoader>()
                .AddTransient<IFilesystem, RealFilesystem>()
                .AddTransient<ProjectWatcher>()
                .AddTransient<IFilesystemWatcher, FilesystemWatcher>()
                .AddTransient<IScheduler>(x => Scheduler.Default)
                .BuildServiceProvider();

            await Task.CompletedTask;

            var rootLogger = serviceProvider.GetRequiredService<ILogger<Program>>();

            rootLogger.LogDebug("Starting ponder at {workingDir} with args {args}", Environment.CurrentDirectory, args);

            var projectPath = args.FirstOrDefault() ?? ".";

            var finder = new ProjectFinder(new RealFilesystem());

            var findResult = finder.FindProject(projectPath);

            if(!findResult.IsFound)
            {
                Console.WriteLine($"Unable to locate csproj file; candidates: {string.Join(", ", findResult.Candidates)}");
                return;
            }

            var project = serviceProvider.GetRequiredService<ProjectLoader>()
                .Load(RelPath.FromString(findResult.ProjectPath!));

            var watcher = new ProjectWatcher(
                project,
                serviceProvider.GetRequiredService<IFilesystemWatcher>(),
                Scheduler.Default,
                serviceProvider.GetRequiredService<ILogger<ProjectWatcher>>());

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
                            .Subscribe(observer!);

                return Disposable.Create(() => {
                    subscription.Dispose();
                    process.StandardInput.Write((char)3);
                });
            });
        }
    }
}
