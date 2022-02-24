using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Ponder.Old
{
    public sealed class PonderEngine<TCompilerResult, TTestResult>
    {

        // todo: reload projects
    }

    public interface IPonderEngineMount<TCompilerResult, TTestResult>
    {
        IObservable<RelPath> WatchProject(Project project);
        IEnumerable<Project> FindRelevantProjects(RelPath filePath, ImmutableArray<Project> projects);

        //IObservable<ImmutableArray<CompilerIssue>> BuildProject(Project project);
        IObservable<TCompilerResult> BuildProject(Project project);
        IObservable<TTestResult> RunTests(Project project);
    }
}