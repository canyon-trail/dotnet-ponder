
using Microsoft.Extensions.DependencyInjection;
using Ponder;
using Ponder.Old;
using Ponder.Old.Exits;
using Ponder.Old.Sln;

var bus = new Bus(new ServiceCollection().AddPonderServices());

if (args.Length > 0)
{
    var path = Path.GetFullPath(args[0]);
    await bus.Publish(new SlnSelected(path));
}
else
{
    var finder = bus.Provider.GetRequiredService<SlnFinder>();
    await finder.FindSln();
}

var exitGate = bus.Provider.GetRequiredService<ExitGate>();

await exitGate.ShouldExit;
