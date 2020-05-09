using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ponder
{
    public sealed class Project
    {
        public Project(string csProjPath, params Project[] references)
        {
            CsProjPath = csProjPath;
            References = references;
        }
        public string CsProjPath { get; }
        public IEnumerable<Project> References { get; }

        public IEnumerable<string> WatchPaths => new[] {
            CsProjPath,
            Path.Combine(Path.GetDirectoryName(CsProjPath)!, "**", "*.cs")
        };

        public IEnumerable<string> WatchFolders =>
            new[] {
                Path.GetDirectoryName(CsProjPath)!
            }.Concat(
                References.SelectMany(x => x.WatchFolders)
            )
            .Distinct();

        public string ProjectFolder => Path.GetDirectoryName(CsProjPath)!;

        public bool IsMatch(string path) =>
            path == CsProjPath || (
                path.StartsWith(ProjectFolder)
                && path.EndsWith(".cs")
            );

        public Project? FindPertinentProject(string path)
        {
            return IsMatch(path)
                ? this
                : References
                    .Select(x => x.FindPertinentProject(path))
                    .FirstOrDefault();
        }
    }
}
