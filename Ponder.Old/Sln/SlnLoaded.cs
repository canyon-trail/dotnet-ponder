using System.Collections.Immutable;
using Ponder.Parsers;

namespace Ponder.Sln;

public sealed record SlnLoaded(ImmutableArray<SlnParser.SlnProject> Projects);
