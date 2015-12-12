using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace Onism.Cldr.Packages
{
    /// <summary>
    /// Represents one of the packages the CLDR data has been grouped into. This is a "smart enum" type.
    /// </summary>
    public abstract partial class CldrPackage
    {
        internal const string Extension = ".cldrpkg";

        /// <summary>
        /// Gets or sets the name of this package.
        /// </summary>
        public string Name { get; protected set; }

        protected CldrPackage(string name)
        {
            Name = $"cldr-{name}";
        }

        /// <summary>
        /// Validates a file extracted from this package and creates a temporary representation
        /// of the data to be later consumed while building <see cref="CldrData"/>.
        /// </summary>
        internal abstract CldrJson TryParseFile(string path);

        /// <summary>
        /// Downloads this CLDR package from GitHub to a local directory.
        /// </summary>
        /// <param name="destinationDirectoryName">The path to the directory in which to place the extracted files.</param>
        public void Download(string destinationDirectoryName)
        {
            using (var client = new WebClient())
            {
                var uri = $"https://github.com/unicode-cldr/{Name}/archive/master.zip";
                var tempPath = Path.GetTempPath();
                var zipPath = Path.Combine(tempPath, $"{Name}.zip");
                var packageDirectoryName = Path.Combine(tempPath, $"{Name}");

                // download the package as a zip and unpack it
                client.DownloadFile(uri, zipPath);
                ZipFile.ExtractToDirectory(zipPath, packageDirectoryName);
                File.Delete(zipPath);

                // parse the package
                var cldrJsons = CldrJsonPathExtractor
                    .ExtractPaths(packageDirectoryName)
                    .Select(TryParseFile)
                    .ToArray();

                // cleanup
                Directory.Delete(packageDirectoryName, true);

                // serialize the collection of CldrJsons
                var resultPath = Path.Combine(destinationDirectoryName, Name + Extension);
                var result = JsonConvert.SerializeObject(cldrJsons, Formatting.Indented);
                File.WriteAllText(resultPath, result);
            }
        }
    }
}
