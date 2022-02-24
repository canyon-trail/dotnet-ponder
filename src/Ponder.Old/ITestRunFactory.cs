using System;

namespace Ponder.Old
{
    public interface ITestRunFactory
    {
        IObservable<string> RunTests(Project project);
    }
}
