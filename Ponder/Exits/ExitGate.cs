namespace Ponder.Exits;

public sealed class ExitGate : IBusListener<ExitSignal>
{
    private readonly TaskCompletionSource _tcs = new();
    public Task ShouldExit => _tcs.Task;

    public void OpenGate()
    {
        _tcs.TrySetResult();
    }

    public void OnPublish(ExitSignal message)
    {
        OpenGate();
    }
}
