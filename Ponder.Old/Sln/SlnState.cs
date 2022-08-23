using System.Collections.Immutable;
using Ponder.Old.Projects;

namespace Ponder.Old.Sln;

public abstract record ProjectState(string Name, string Path);

public sealed record LoadingProjectState(string Name, string Path)
    : ProjectState(Name, Path);

public sealed record LoadedProjectState(ProjectInfo Project)
    : ProjectState(Project.Name, Project.Path);

public record SlnState(ImmutableArray<ProjectState> Projects);
