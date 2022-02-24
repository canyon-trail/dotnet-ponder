using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ponder.Old
{
    public sealed class Project
    {
        public Project(RelPath csProjPath, params Project[] references)
        {
            CsProjPath = csProjPath;
            References = references;
        }
        public RelPath CsProjPath { get; }
        public IEnumerable<Project> References { get; }
        public RelPath ProjectFolder => CsProjPath.Parent;

        public IEnumerable<RelPath> WatchPaths => new[] {
            CsProjPath,
            ProjectFolder.Append("**", "*.cs")
        };

        public IEnumerable<RelPath> WatchFolders =>
            new[] {
                ProjectFolder
            }.Concat(
                References.SelectMany(x => x.WatchFolders)
            )
            .Distinct();


        public bool IsMatch(RelPath path) =>
            FindPertinentProject(path) != null;

        public Project? FindPertinentProject(RelPath path)
        {
            return IsLocalMatch(path)
                ? this
                : References
                    .Select(x => x.FindPertinentProject(path))
                    .FirstOrDefault();
        }

        private bool IsLocalMatch(RelPath path) =>
            path.IsSameAs(CsProjPath) || (
                path.IsChildPathOf(ProjectFolder)
                && path.Segments.Last().EndsWith(".cs")
            );
    }
}
