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

        public Project Load(RelPath projectFilePath)
        {
            return ParseProject(projectFilePath, RelPath.FromString(""));
        }

        private Project ParseProject(RelPath projectFilePath, RelPath root)
        {
            _logger.LogWarning("Parsing project at {path}", projectFilePath.Path);
            var projectFileXml = XElement.Parse(_filesystem.ReadFile(projectFilePath.Path));

            var projectFolder = projectFilePath.Parent;

            var otherProjects = projectFileXml
                .Descendants("ProjectReference")
                .Select(x => x.Attribute("Include").Value)
                .Select(x => RelPath.FromString(x).RelativeTo(projectFolder).RelativeTo(root))
                .Select(x => ParseProject(x, root))
                .ToArray();

            return new Project(projectFilePath, otherProjects);
        }
    }
}