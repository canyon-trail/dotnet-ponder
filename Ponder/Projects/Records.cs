using System.Collections.Immutable;

namespace Ponder.Projects;

public record ProjectInfo(string Name, string Path, ImmutableArray<string> References, bool IsTestProject);
public record ProjectLoaded(ProjectInfo Project);

public record ProjectIgnored(string Path);
