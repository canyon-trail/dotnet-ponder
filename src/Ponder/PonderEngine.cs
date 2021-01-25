using System;
using System.Collections.Immutable;

namespace Ponder
{
    public sealed class PonderEngine
    {

        // todo: reload projects
    }

    public interface IPonderEngineMount
    {
        IObservable<string> WatchProject(Project project);
        ImmutableArray<CompilerIssue> BuildProject(Project project);
    }

    public sealed class TestSuccess
    {
        public TestSuccess(string name)
        {
            Name = name;
        }
        public string Name { get; }
    }
    public sealed class
}