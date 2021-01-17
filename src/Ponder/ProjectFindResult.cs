using System.Collections.Generic;

namespace Ponder
{
    public sealed class ProjectFindResult
    {
        public string? ProjectPath { get; set; }
        public bool IsFound { get; set; }
        public IEnumerable<string> Candidates { get; set; } = new string[0];
    }

}