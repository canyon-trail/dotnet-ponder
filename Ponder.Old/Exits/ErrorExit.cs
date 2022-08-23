namespace Ponder.Old.Exits;

public sealed class ErrorExit
    : IBusListener<ErrorMessageAndExitSignal>
{
    private readonly IBus _bus;

    public ErrorExit(IBus bus)
    {
        _bus = bus;
    }
    public Task OnPublish(ErrorMessageAndExitSignal message)
    {
        Console.Error.WriteLine(message.Message);

        _bus.Publish(new ExitSignal());

        return Task.CompletedTask;
    }
}
