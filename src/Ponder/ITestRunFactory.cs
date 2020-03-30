using System;

namespace Ponder
{
    public interface ITestRunFactory
    {
        IObservable<string> RunTests(Project project);
    }
}
