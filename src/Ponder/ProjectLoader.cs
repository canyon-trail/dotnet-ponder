using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace Ponder
{
    public sealed class ProjectLoader
    {
        private readonly IFilesystem _filesystem;
        private readonly ILogger<ProjectLoader> _logger;

        public ProjectLoader(IFilesystem filesystem, ILogger<ProjectLoader> logger)
        {
            _filesystem = filesystem;
            _logger = logger;
        }

        public Project Load(string projectFilePath)
        {
            var root = Directory.GetCurrentDirectory();

            return ParseProject(projectFilePath, root);
        }

        private Project ParseProject(string projectFilePath, string root)
        {
            _logger.LogWarning("Parsing project at {path}", projectFilePath);
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