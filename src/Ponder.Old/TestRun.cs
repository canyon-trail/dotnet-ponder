using System;
using System.Collections.Immutable;

namespace Ponder.Old
{
    public sealed class TestRun
    {
        public TestRun(int testCount, ImmutableArray<TestFailure> failures, TimeSpan totalTime)
        {
            TestCount = testCount;
            Failures = failures;
            TotalTime = totalTime;
        }
        public int TestCount { get; }
        public ImmutableArray<TestFailure> Failures { get; }
        public TimeSpan TotalTime { get; }
    }
}