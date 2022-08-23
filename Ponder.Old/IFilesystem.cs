using System.Collections.Immutable;
using System.Xml.Linq;
using Ponder.Parsers;
using Ponder.Projects;

namespace Ponder;

public interface IFilesystem
{
    string CurrentDirectory { get; }
    Task<ImmutableArray<string>> ListFiles(string directory, string filter);
    Task<SlnParser.SlnFile> LoadSln(string path);
    Task<Projects.ProjectInfo> LoadProject(string path);
    bool Exists(string path);
    void SetDirectory(string path);
}

public sealed class RealFilesystem : IFilesystem
{
    public string CurrentDirectory => Directory.GetCurrentDirectory();
    public Task<ImmutableArray<string>> ListFiles(string directory, string filter)
    {
        return Task.FromResult(
            ImmutableArray.Create(
                Directory.GetFiles(directory, filter)
            )
        );
    }

    public async Task<SlnParser.SlnFile> LoadSln(string path)
    {
        var slnLines = await File.ReadAllLinesAsync(path);

        return SlnParser.parseSlnFromLines(slnLines);
    }

    public async Task<ProjectInfo> LoadProject(string path)
    {
        var xmlRoot = XElement.Parse(await File.ReadAllTextAsync(path));

        var projectFolder = Path.GetDirectoryName(path);

        var otherProjects = xmlRoot
            .Descendants("ProjectReference")
            .Select(x => x.Attribute("Include")?.Value!)
            .Select(x => Path.GetFullPath(Path.Combine(projectFolder!, x)))
            .ToArray();

        var isTest = xmlRoot
            .Descendants("PackageReference")
            .Any(x => x.Attribute("Include")?.Value == "Microsoft.NET.Test.Sdk");

        return new ProjectInfo(Path.GetFileNameWithoutExtension(path), path, otherProjects.ToImmutableArray(), isTest);
    }

    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public void SetDirectory(string path)
    {
        Directory.SetCurrentDirectory(path);
    }
}
