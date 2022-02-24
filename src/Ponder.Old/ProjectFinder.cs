using System.Linq;

namespace Ponder.Old
{
    public sealed class ProjectFinder
    {
        private readonly IFilesystem _filesystem;

        public ProjectFinder(IFilesystem filesystem)
        {
            _filesystem = filesystem;
        }

        public ProjectFindResult FindProject(string path)
        {
            return TryFindByFullPath(path)
                ?? FindByDirectory(path);
        }

        private ProjectFindResult? TryFindByFullPath(string path)
        {
            if(!path.EndsWith(".csproj"))
            {
                return null;
            }

            return new ProjectFindResult
            {
                ProjectPath = path,
                IsFound = _filesystem.FileExists(path),
                Candidates = new[] { path },
            };
        }

        private ProjectFindResult FindByDirectory(string path)
        {
            var results = _filesystem.GetCsProjFiles(path);

            var success = results.Count() == 1;

            return new ProjectFindResult
            {
                ProjectPath = success ? results.Single() : null,
                IsFound = success,
                Candidates = results,
            };
        }
    }
}