using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.Utils;

//  There is some information you need to know about the CLDR JSON structure.
//
//  This data is meant to be delivered using Bower (a package manager for Javascript libraries).
//  Thus, each CLDR package (in practice, a GitHub repository) contains one bower.json file in its
//  main directory. What kind of information does it provide? The name of the package, its version, dependencies, certain
//  files to be ignored, and - most importantly - the "main" property, which lists "the primary acting files
//  necessary to use the package". Therefore Onism seeks for such bower.json files to extract that information.
//  Once they are all discovered, paths to the relevant files can be resolved.
// 
//  The paths described by the "main" property in a bower.json file use certain patterns. As for December 2015,
//  the CLDR JSON data uses only these paths:
//
//      - main/**/*.json
//      - segments/**/*.json
//      - availableLocales.json
//      - defaultContent.json
//      - supplemental/*.json
//   
//  The specification of Bower does not define what kind of patterns are used in this specific property. It is hard
//  to guess if those files will introduce new patterns in the future and what exactly will be delivered by Unicode.
//  The code below simply splits such paths into separate components, treating each of them as a valid search pattern
//  to be consumed directly by Directory.GetFiles() and Directory.GetDirectories(). According to the specification of those methods,
//  such pattern can be a combination of literal and wildcard characters, but doesn't support regular expressions.
//  The only allowed wildcard characters are '*' and '?'. Characters other than the wildcard are literal characters.

namespace Onism.Cldr.Packages
{
    /// <summary>
    /// Utility class for extracting relevant CLDR JSON file paths from packages using bower.json files.
    /// </summary>
    internal static class CldrJsonPathExtractor
    {
        /// <summary>
        /// Extracts relevant file paths from the specified directory.
        /// </summary>
        /// <param name="path">The path to the directory to search.</param>
        public static IEnumerable<string> ExtractPaths(string path)
        {
            var bowerFiles = GetBowerFiles(path);
            var extracted = new List<string>();

            foreach (var bowerFile in bowerFiles)
            {
                var token = JsonUtils.LoadFromFile(bowerFile)["main"];

                var patterns = token.Type == JTokenType.Array
                    ? token.Values().Select(x => (string)x)
                    : new[] { (string)token };

                var bowerDirectory = Path.GetDirectoryName(bowerFile);
                extracted.AddRange(patterns.SelectMany(x => ResolvePattern(bowerDirectory, x)));
            }

            // if wildcard-based pattern overlaps with explicit paths,
            // there are duplicates
            return extracted.Distinct();
        }

        /// <summary>
        /// Returns the paths to Bower files found in the current directory and its subdirectories.
        /// </summary>
        /// <param name="path">The path to the directory to search.</param>
        private static IEnumerable<string> GetBowerFiles(string path)
        {
            var bowerFiles = new List<string>();
            var bowerFile = Path.Combine(path, "bower.json");

            if (File.Exists(bowerFile))
            {
                bowerFiles.Add(bowerFile);

                // a bower.json file was found here, so there is no chance
                // of finding another (valid) one in a subdirectory
                return bowerFiles;
            }

            foreach (var dir in Directory.GetDirectories(path))
                bowerFiles.AddRange(GetBowerFiles(dir));

            return bowerFiles;
        }

        private static IEnumerable<string> ResolvePattern(string path, string pattern)
        {
            // at each level all the paths will be enhanced
            // starting from the input path, which is the directory with bower.json
            var paths = path.Yield().ToArray();
            var patternSegments = pattern.Split('/');
            var steps = patternSegments.Length;

            // all the segments except the last one
            var segments = patternSegments.Take(steps - 1).ToArray();
            var lastSegment = patternSegments.Last();

            // go deeper
            foreach (var segment in segments)
                paths = paths.SelectMany(x => Directory.GetDirectories(x, segment)).ToArray();

            // no more subdirectories to visit, files are here
            return paths.SelectMany(x => Directory.GetFiles(x, lastSegment));
        }
    }
}