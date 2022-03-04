
var slnPath = (string) null!;

if (args.Length > 0)
{
    slnPath = args[0];
    if (!File.Exists(slnPath))
    {
        Console.WriteLine($"File {slnPath} not found;" +
        " run in a directory with a single sln file " +
        "or specify the path to the sln");
    }
}
else
{
    var candidates = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.sln");

    if (candidates.Length == 0)
    {
        Console.WriteLine("Unable to find sln file;" +
        " run in a directory with a single sln file " +
        "or specify the path to the sln");
        return;
    }

    if (candidates.Length > 1)
    {
        Console.WriteLine("Found multiple sln files;" +
        " run in a directory with a single sln file " +
        "or specify the path to the sln");
        return;
    }

    slnPath = candidates[0];
}

await using var slnStream = File.OpenRead(slnPath);

var slnFile = await Ponder.Parsers.SlnParser.parseSlnFromStream(slnStream);

Console.WriteLine("Projects:");
foreach (var project in slnFile.Projects)
{
    Console.WriteLine($"\t{project.Name}: {project.Path}");
}
