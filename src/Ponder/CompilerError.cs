namespace Ponder
{
    public sealed class CompilerIssue
    {
        public string Message { get; }
        public string ErrorCode { get; }
        public FilePosition? Position { get; }
        public CompilerIssueType IssueType { get; }

        public static CompilerIssue Warning(string message, string errorCode, FilePosition? position = null)
        {
            return new CompilerIssue(message, errorCode, CompilerIssueType.Warning, position);
        }
        public static CompilerIssue Error(string message, string errorCode, FilePosition? position = null)
        {
            return new CompilerIssue(message, errorCode, CompilerIssueType.Warning, position);
        }

        private CompilerIssue(string message, string errorCode, CompilerIssueType issueType, FilePosition? position)
        {
            Message = message;
            ErrorCode = errorCode;
            IssueType = issueType;
            Position = position;
        }
    }
}