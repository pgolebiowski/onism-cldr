using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Onism.Cldr
{
    /// <summary>
    /// Represents a package containing supplementary information about locales.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class CldrSupplementalPackage : CldrPackage
    {
        [JsonProperty]
        private JObject _json;

        internal static string BaseExtension => ".cldrsup";

        internal override string Extension => BaseExtension;

        internal CldrSupplementalPackage(string name)
            : base(name)
        {
            
        }

        internal override void TryParsePackage(string directoryPath)
        {
            _json = CldrPackagePathExtractor
                .ExtractPaths(directoryPath)
                .Select(FormatFile)
                .Aggregate(JsonMerger.SafeMerge);
        }

        private static JObject FormatFile(string path)
        {
            var json = File.ReadAllText(path);

            // root
            var o = JObject.Parse(json)
                .PropertiesCountShouldBe(1);

            if (o.Property("availableLocales") != null || o.Property("defaultContent") != null)
                return o;

            // supplemental
            o.PropertiesShouldContain("supplemental", JTokenType.Object);

            var supplemental = ((JObject) o["supplemental"])
                .PropertiesCountShouldBe(2)
                .PropertiesShouldContain("version", JTokenType.Object);

            supplemental.Property("version").Remove();

            return o;
        }

        internal override string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}