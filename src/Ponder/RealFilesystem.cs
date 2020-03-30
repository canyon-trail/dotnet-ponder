using System.Collections.Generic;
using System.IO;

namespace Ponder
{
    public sealed class RealFilesystem : IFilesystem
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public IEnumerable<string> GetCsProjFiles(string dir)
        {
            return Directory.GetFiles(dir, "*.csproj");
        }
    }
}
