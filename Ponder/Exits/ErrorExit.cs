namespace Ponder.Exits;

public sealed class ErrorExit
    : IBusListener<ErrorMessageAndExitSignal>
{
    private readonly IBus _bus;

    public ErrorExit(IBus bus)
    {
        _bus = bus;
    }
    public void OnPublish(ErrorMessageAndExitSignal message)
    {
        Console.Error.WriteLine(message.Message);

        _bus.Publish(new ExitSignal());
    }
}
