using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Ponder
{
    public sealed class ProjectLoader
    {
        private readonly IFilesystem _filesystem;

        public ProjectLoader(IFilesystem filesystem)
        {
            _filesystem = filesystem;
        }

        public Project Load(string projectFilePath)
        {
            var root = Directory.GetCurrentDirectory();

            return ParseProject(projectFilePath, root);
        }

        private Project ParseProject(string projectFilePath, string root)
        {
            var projectFileXml = XElement.Parse(_filesystem.ReadFile(projectFilePath));

            var projectFolder = Path.GetFullPath(Path.Join(root, Path.GetDirectoryName(projectFilePath)));

            var otherProjects = projectFileXml
                .Descendants("ProjectReference")
                .Select(x => x.Attribute("Include").Value)
                .Select(x => Path.GetRelativePath(root, Path.GetFullPath(Path.Join(projectFolder, x))))
                .Select(x => ParseProject(x, root))
                .ToArray();

            return new Project(projectFilePath, otherProjects);
        }
    }
}