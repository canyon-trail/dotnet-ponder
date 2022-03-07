using System.Collections.Immutable;

namespace Ponder;

public interface IFilesystem
{
    string CurrentDirectory { get; }
    Task<ImmutableArray<string>> ListFiles(string directory, string filter);
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
}
