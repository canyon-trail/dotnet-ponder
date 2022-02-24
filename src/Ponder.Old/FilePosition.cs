namespace Ponder.Old
{
    public sealed class FilePosition
    {
        public FilePosition(string filename, int lineNumber, int? column)
        {
            Filename = filename;
            LineNumber = lineNumber;
            Column = column;
        }
        public string Filename { get; }
        public int LineNumber { get; }
        public int? Column { get; }
    }
}