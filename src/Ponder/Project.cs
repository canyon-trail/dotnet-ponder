using System.Collections.Generic;
using System.IO;

namespace Ponder
{
    public sealed class Project
    {
        public Project(string csProjPath)
        {
            CsProjPath = csProjPath;
        }
        public string CsProjPath { get; }

        public IEnumerable<string> WatchPaths => new[] {
            CsProjPath,
            Path.Combine(Path.GetDirectoryName(CsProjPath), "**", "*.cs")
        };

        public string WatchFolder => Path.GetDirectoryName(CsProjPath);

        public bool IsMatch(string path) =>
            path == CsProjPath || (
                path.StartsWith(WatchFolder)
                && path.EndsWith(".cs")
            );
    }
}
