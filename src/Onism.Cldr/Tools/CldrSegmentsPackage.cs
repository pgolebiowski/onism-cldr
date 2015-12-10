using System.IO;
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
        internal override CldrJson TryParseFile(string path)
        {
            var localeCode = Path.GetFileName(Path.GetDirectoryName(path));
            var json = File.ReadAllText(path);

            // root
            var o = JObject.Parse(json)
                .PropertiesCountShouldBe(1)
                .PropertiesShouldContain("segments", JTokenType.Object);

            // segments
            var segments = ((JObject) o["segments"])
                .PropertiesCountShouldBe(2)
                .PropertiesShouldContain("identity", JTokenType.Object)
                .PropertiesShouldContain("segmentations", JTokenType.Object);

            // extract the information
            var identity = segments["identity"]
                .ToObject<CldrLocale>()
                .LocaleCodeShouldBe(localeCode);

            var segmentations = new JObject(segments.Property("segmentations"));

            return new CldrJson(GetType(), identity, segmentations);
        }
    }
}