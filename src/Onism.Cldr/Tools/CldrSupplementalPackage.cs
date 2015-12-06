using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;

namespace Onism.Cldr.Tools
{
    /// <summary>
    /// Represents a package containing supplementary information about locales.
    /// </summary>
    public sealed class CldrSupplementalPackage : CldrPackage
    {
        internal CldrSupplementalPackage(string name)
            : base(name)
        {
            
        }

        internal override void TryParsePackage(string directoryPath)
        {
            var merged = CldrPackagePathExtractor
                .ExtractPaths(directoryPath)
                .Select(FormatFile)
                .ToArray()
                .HierarchicalAggregate(JsonMerger.SafeMerge);

            Data = new CldrJson(CldrLocale.None, merged)
                .Yield()
                .ToArray();
        }

        private static JObject FormatFile(string path)
        {
            var json = File.ReadAllText(path);

            // root
            var o = JObject.Parse(json)
                .PropertiesCountShouldBe(1);
            
            // files left as they are
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
    }
}