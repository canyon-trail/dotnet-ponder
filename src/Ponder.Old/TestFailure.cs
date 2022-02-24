using System;

namespace Ponder.Old
{
    public sealed class TestFailure
    {
        public TestFailure(string testName, StackTrace stackTrace, TimeSpan executionTime)
        {
            TestName = testName;
            StackTrace = stackTrace;
            ExecutionTime = executionTime;
        }

        public string TestName { get; }

        public StackTrace StackTrace { get; }
        public TimeSpan ExecutionTime { get; }
    }
}