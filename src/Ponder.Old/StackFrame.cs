namespace Ponder.Old
{
    public sealed class StackFrame
    {
        public StackFrame(string methodName, FilePosition? position = null)
        {
            MethodName = methodName;
            Position = position;
        }

        public string MethodName { get; }

        public FilePosition? Position { get; }
    }
}