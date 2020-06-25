using io = System.IO;

namespace Ponder
{
    public sealed class RelPath
    {
        public string Path { get; }
        public RelPath(string path)
        {
            Path = path;
        }

        public RelPath RelativeTo(RelPath startPoint)
        {
            return new RelPath(startPoint.Path + io.Path.DirectorySeparatorChar + Path);
        }
    }
}