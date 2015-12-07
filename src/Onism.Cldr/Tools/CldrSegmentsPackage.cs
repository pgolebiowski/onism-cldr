using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr.Tools
{
    /// <summary>
    /// Represents a package containing line breaking data from Unicode's ULI project.
    /// </summary>
    public sealed class CldrSegmentsPackage : CldrPackage
    {
        internal CldrSegmentsPackage(string name)
            : base(name)
        {

        }

        /// <summary>
        /// Validates files discovered in this package and creates a temporary
        /// representation of the data to be later consumed while building a <see cref="CldrTree"/>.
        /// </summary>
        internal override IEnumerable<CldrJson> TryParsePackage(string directoryPath)
        {
            return (from path in CldrPackagePathExtractor.ExtractPaths(directoryPath)

                    let localeCode = Path.GetFileName(Path.GetDirectoryName(path))
                    let json = File.ReadAllText(path)

                    // root
                    let o = JObject.Parse(json)
                        .PropertiesCountShouldBe(1)
                        .PropertiesShouldContain("segments", JTokenType.Object)

                    // segments
                    let segments = ((JObject)o["segments"])
                        .PropertiesCountShouldBe(2)
                        .PropertiesShouldContain("identity", JTokenType.Object)
                        .PropertiesShouldContain("segmentations", JTokenType.Object)

                    // extract the information
                    let identity = segments["identity"]
                        .ToObject<CldrLocale>()
                        .LocaleCodeShouldBe(localeCode)

                    let segmentations = new JObject(segments.Property("segmentations"))

                    select new CldrJson(GetType(), identity, segmentations))
                .ToArray();
        }
    }
}