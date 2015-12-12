using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.Tools;
using ProtoBuf;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents unified CLDR data.
    /// </summary>
    [ProtoContract]
    public class CldrData
    {
        /// <summary>
        /// <see cref="CldrTree"/> object containing localization data.
        /// </summary>
        [ProtoMember(1)]
        public CldrTree Standard { get; }

        /// <summary>
        /// Serialized JSON object containing supplemental CLDR data, merged in a safe way.
        /// Use your favourite tool to browse this object.
        /// </summary>
        [ProtoMember(2)]
        public string Supplemental { get; }

        /// <summary>
        /// Serialized JSON object containing line breaking data from Unicode's ULI project,
        /// merged in a safe way. Use your favourite tool to browse this object.
        /// </summary>
        [ProtoMember(3)]
        public string Segments { get; }


        private CldrData(CldrTree standard, string supplemental, string segments)
        {
            Standard = standard;
            Supplemental = supplemental;
            Segments = segments;
        }

        /// <summary>
        /// Writes a protocol-buffer representation of the given instance to the supplied stream.
        /// </summary>
        /// <param name="destination">The destination stream to write to.</param>
        public void Serialize(Stream destination)
        {
            Serializer.Serialize(destination, this);
        }

        /// <summary>
        /// Creates a new <see cref="CldrData"/> instance from a protocol-buffer stream.
        /// </summary>
        /// <param name="source">The binary stream to apply to the new instance (cannot be null).</param>
        public static CldrData Deserialize(Stream source)
        {
            return Serializer.Deserialize<CldrData>(source);
        }

        /// <summary>
        /// Downloads the specified CLDR packages from GitHub and merges them.
        /// </summary>
        /// <param name="packages">The packages to be downloaded and merged.</param>
        public static CldrData MergePackages(IEnumerable<CldrPackage> packages)
        {
            var tempPath = Path.GetTempPath();
            var packagesDirectory = Path.Combine(tempPath, "cldr-packages");
            Directory.CreateDirectory(packagesDirectory);

            packages.ForEach(x => x.Download(packagesDirectory));
            var result = MergePackages(packagesDirectory);
            Directory.Delete(packagesDirectory, true);

            return result;
        }

        /// <summary>
        /// Searches the specified directory, looking for CLDR package files.
        /// Once found, their data is merged.
        /// </summary>
        /// <param name="directoryName">The relative or absolute path to the directory to search.</param>
        public static CldrData MergePackages(string directoryName)
        {
            var packages = Directory.GetFiles(directoryName, "*.cldrpkg");
            return MergePackages(packages);
        }

        /// <summary>
        /// Merges the data found in the specified CLDR package files.
        /// </summary>
        /// <param name="paths">The paths to package files to be merged.</param>
        public static CldrData MergePackages(string[] paths)
        {
            // these objects will be modified untill
            // all packages are parsed
            var standard = new CldrTree();
            var supplemental = new JObject();
            var segments = new JObject();

            // "smart switch" type
            var @switch = new Dictionary<Type, Action<CldrJson>>
            {
                {
                    typeof(CldrStandardPackage),
                    json => standard.Add(json)
                },
                {
                    typeof(CldrSupplementalPackage),
                    json => JsonUtils.SafeMerge(supplemental, json.Data)
                },
                {
                    typeof(CldrSegmentsPackage),
                    json => JsonUtils.SafeMerge(segments, json.Data)
                }
            };

            // by design, package files are in fact collections of CldrJson objects.
            // thus, it is sufficient to read all package files, extract those CldrJson
            // objects from them and aggregate their data properly (the switch above)
            paths
                .Select(File.ReadAllText)
                .SelectMany(JsonConvert.DeserializeObject<CldrJson[]>)
                .ForEach(cldrJson => @switch[cldrJson.Package](cldrJson));

            // all the packages have already been parsed
            return new CldrData(standard, supplemental.ToString(), segments.ToString());
        }
    }
}
