using System.IO;
using Newtonsoft.Json.Linq;

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

        internal override CldrJson TryParseFile(string path)
        {
            var json = File.ReadAllText(path);

            // root
            var o = JObject.Parse(json)
                .PropertiesCountShouldBe(1);

            // files left as they are
            if (o.Property("availableLocales") == null && o.Property("defaultContent") == null)
            {
                // supplemental
                o.PropertiesShouldContain("supplemental", JTokenType.Object);

                var supplemental = ((JObject) o["supplemental"])
                    .PropertiesCountShouldBe(2)
                    .PropertiesShouldContain("version", JTokenType.Object);
            }

            return new CldrJson(GetType(), CldrLocale.None, o);
        }
    }
}