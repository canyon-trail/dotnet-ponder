
using Microsoft.Extensions.DependencyInjection;
using Ponder;
using Ponder.Exits;

var bus = new Bus(new ServiceCollection().AddPonderServices());

if (args.Length > 0)
{
    bus.Publish(new SlnSelected(args[0]));
}
else
{
    var finder = bus.Provider.GetRequiredService<SlnFinder>();
    await finder.FindSln();
}

var exitGate = bus.Provider.GetRequiredService<ExitGate>();

await exitGate.ShouldExit;
