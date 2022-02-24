using System.Collections.Immutable;

namespace Ponder.Old
{
    public sealed class StackTrace
    {
        public string ExceptionType { get; }

        public StackTrace(string exceptionType, string exceptionMessage, ImmutableArray<StackFrame> frames)
        {
            ExceptionType = exceptionType;
            ExceptionMessage = exceptionMessage;
            Frames = frames;
        }

        public string ExceptionMessage { get; }

        public ImmutableArray<StackFrame> Frames { get; }
    }
}