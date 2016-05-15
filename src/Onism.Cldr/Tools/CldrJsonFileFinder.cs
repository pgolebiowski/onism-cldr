using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Onism.Cldr.Tools
{
    internal class CldrJsonFileFinder
    {
        public IEnumerable<string> FindFiles(string directory)
        {
            return Directory
                .EnumerateFiles(directory, "*.json", SearchOption.AllDirectories)
                .Where(IsCldrJsonFile);
        }

        private static bool IsCldrJsonFile(string path)
        {
            var fileName = Path.GetFileNameWithoutExtension(path);
            return FilesToIgnore.Contains(fileName) == false;
        }

        private static readonly HashSet<string> FilesToIgnore = new HashSet<string>
        {
            "bower",
            "package"
        };
    }
}
