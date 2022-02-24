using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using io = System.IO;
using PathArray = System.Collections.Immutable.ImmutableArray<string>;

namespace Ponder.Old
{
    public sealed class RelPath
    {
        public static RelPath Empty => new RelPath(ImmutableArray<string>.Empty);
        public string Path =>
            Segments.Any()
                ? string.Join(io.Path.DirectorySeparatorChar, Segments)
                : ".";
        public PathArray Segments { get; }

        public RelPath Parent => new RelPath(Segments.Take(Segments.Length - 1).ToImmutableArray());

        public static RelPath FromString(string path)
        {
            var segments = path
                .Split("/")
                .SelectMany(x => x.Split("\\"))
                .Where(x => !string.IsNullOrEmpty(x))
                .Where(x => x != ".")
                ;

            return segments.Aggregate(Empty, (left, right) => left.Append(right));
        }

        private RelPath(IEnumerable<string> segments)
        {
            Segments = segments
                .Where(x => !string.IsNullOrEmpty(x))
                .Where(x => x != ".")
                .ToImmutableArray();
        }

        private RelPath(string segment)
        {
            Segments = ImmutableArray.Create(segment);
        }

        public RelPath RelativeTo(RelPath startPoint)
        {
            var segments = CollapseRelPaths(startPoint.Segments, Segments);

            return new RelPath(segments);
        }

        public RelPath Append(RelPath childPath)
        {
            var segments = CollapseRelPaths(Segments, childPath.Segments);

            return new RelPath(segments);
        }

        public RelPath Append(params string[] segments)
        {
            return new RelPath(CollapseRelPaths(Segments, segments.ToImmutableArray()));
        }

        private static PathArray CollapseRelPaths(PathArray left, PathArray right)
        {
            while(EndsWithConcretePath(left) && StartsWithRelParent(right))
            {
                left = left.Take(left.Length - 1).ToImmutableArray();
                right = right.Skip(1).ToImmutableArray();
            }

            return left.Concat(right).ToImmutableArray();
        }

        private static bool EndsWithConcretePath(IEnumerable<string> segments)
        {
            return segments.Any() && segments.Last() != "..";
        }

        private static bool StartsWithRelParent(IEnumerable<string> segments)
        {
            return segments.Any() && segments.First() == "..";
        }

        public bool IsSameAs(RelPath other)
        {
            return JoinSegments(Segments).Equals(JoinSegments(other.Segments));
        }

        public bool IsChildPathOf(RelPath parent)
        {
            if(parent.Segments.Length > Segments.Length)
            {
                return false;
            }

            return parent.Segments
                .Zip(
                    Segments.Take(parent.Segments.Length),
                    (left, right) => left.Equals(right))
                .All(x => x);
        }

        private static string JoinSegments(PathArray segments)
        {
            return string.Join(io.Path.DirectorySeparatorChar, segments);
        }

        public override bool Equals(object? obj)
        {
            var other = obj as RelPath;

            return Path.Equals(other?.Path);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Path);
        }

        public override string ToString() => Path;
    }
}