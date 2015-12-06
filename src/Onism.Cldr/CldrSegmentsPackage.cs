using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents a package containing line breaking data from Unicode's ULI project.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CldrSegmentsPackage : CldrPackage
    {
        [JsonProperty]
        private JObject _json;

        internal static string BaseExtension => ".cldrseg";

        internal override string Extension => BaseExtension;

        internal CldrSegmentsPackage(string name)
            : base(name)
        {

        }

        /// <summary>
        /// Validates files discovered in this package and creates a temporary
        /// representation of the data to be later consumed while building a <see cref="CldrTree"/>.
        /// </summary>
        internal override void TryParsePackage(string directoryPath)
        {
            _json = (from path in CldrPackagePathExtractor.ExtractPaths(directoryPath)

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

                     let segmentations = segments.Property("segmentations")

                     // build a new JObject to be aggregated
                     let tmp = new JObject
                      {
                          { localeCode, new JObject(segmentations) }
                      }

                     select tmp)
                     .ToArray()
                     .HierarchicalAggregate(JsonMerger.SafeMerge);
        }

        internal override string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}