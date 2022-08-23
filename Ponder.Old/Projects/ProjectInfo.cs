using System.Collections.Immutable;

namespace Ponder.Old.Projects;

public record ProjectInfo(string Name, string Path, ImmutableArray<string> References, bool IsTestProject);
