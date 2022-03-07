using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Ponder.Exits;
using Xunit;

namespace Ponder.Tests;

public sealed class ExitGateTests
{
    [Fact]
    public async Task GateClosed()
    {
        var testee = new ExitGate();

        await AssertGateClosed(testee);
    }

    [Fact]
    public async Task GateOpen()
    {
        var testee = new ExitGate();

        testee.OpenGate();

        await AssertGateOpen(testee);
    }

    [Fact]
    public async Task OpensOnBusPublish()
    {
        var bus = new Bus(new ServiceCollection().AddPonderServices());

        var gate = bus.Provider.GetRequiredService<ExitGate>();

        await AssertGateClosed(gate);

        bus.Publish(new ExitSignal());

        await AssertGateOpen(gate);
    }

    private static async Task AssertGateClosed(ExitGate testee)
    {
        var delay = Task.Delay(5);
        var res = await Task.WhenAny(delay, testee.ShouldExit);

        res.Should().Be(delay);
    }

    private static async Task AssertGateOpen(ExitGate testee)
    {
        var delay = Task.Delay(5);
        var res = await Task.WhenAny(delay, testee.ShouldExit);

        res.Should().Be(testee.ShouldExit);
    }
}
