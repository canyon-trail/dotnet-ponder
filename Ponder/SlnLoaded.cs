using System.Collections.Immutable;
using Ponder.Parsers;

namespace Ponder;

public sealed record SlnLoaded(ImmutableArray<SlnParser.SlnProject> Projects);