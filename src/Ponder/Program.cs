using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ponder
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Task.CompletedTask;

            var currentDir = Directory.GetCurrentDirectory();
            var csprojFiles = Directory.GetFiles(currentDir, "*.csproj");

            Console.WriteLine($"Found project file: {csprojFiles.First()}");
        }
    }
}
