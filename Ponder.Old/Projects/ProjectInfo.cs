using System.Collections.Immutable;

namespace Ponder.Projects;

public record ProjectInfo(string Name, string Path, ImmutableArray<string> References, bool IsTestProject);
