using System.Collections.Immutable;

namespace Ponder.Old.Sln;

public sealed record SlnLoaded(ImmutableArray<SlnTypes.SlnProject> Projects);
