using System.IO;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Utils;

namespace Onism.Cldr.Packages
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
        /// Validates a file extracted from this package and creates a temporary representation
        /// of the data to be later consumed while building a <see cref="CldrTree"/>.
        /// </summary>
        internal override CldrJson TryParseFile(string path)
        {
            var localeCode = Path.GetFileName(Path.GetDirectoryName(path));

            // root
            var o = JsonUtils.LoadFromFile(path)
                .DescendantsTypesShouldOnlyBe(JTokenType.Object, JTokenType.Property, JTokenType.String, JTokenType.Array)
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