namespace Ponder.Exits;

public sealed record ExitSignal { }

public sealed record ErrorMessageAndExitSignal(string Message);
