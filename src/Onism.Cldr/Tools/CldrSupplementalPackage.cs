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

        /// <summary>
        /// Validates a file extracted from this package and creates a temporary representation
        /// of the data to be later consumed while building a <see cref="CldrTree"/>.
        /// </summary>
        internal override CldrJson TryParseFile(string path)
        {
            // root
            var o = JsonUtils.LoadFromFile(path)
                .PropertiesCountShouldBe(1);

            // files left as they are
            if (o.Property("availableLocales") == null && o.Property("defaultContent") == null)
            {
                // supplemental
                o.PropertiesShouldContain("supplemental", JTokenType.Object);

                ((JObject) o["supplemental"])
                    .PropertiesCountShouldBe(2)
                    .PropertiesShouldContain("version", JTokenType.Object);
            }

            return new CldrJson(GetType(), CldrLocale.None, o);
        }
    }
}