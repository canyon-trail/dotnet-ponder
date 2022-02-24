using System.Collections.Generic;

namespace Ponder.Old
{
    public interface IFilesystem
    {
        IEnumerable<string> GetCsProjFiles(string dir);
        bool FileExists(string path);

        string ReadFile(string path);
    }

}