using Newtonsoft.Json.Linq;
using Onism.Cldr.Tools.JsonParsers.Schemas;

namespace Onism.Cldr.Tools.JsonParsers
{
    /// <summary>
    /// Parses JSONs valid against the "Supplemental" schema.
    /// </summary>
    internal sealed class SupplementalParser : CldrJsonParser
    {
        public SupplementalParser() : base(CldrJsonSchemas.Supplemental)
        {
        }

        public override CldrJsonMetadata ExtractMetadata(JObject obj)
        {
            return new CldrJsonMetadata
            {
                CldrVersion = obj.SelectToken("supplemental.version._cldrVersion").ToString()
            };
        }

        public override void RemoveMetadata(JObject obj)
        {
            obj.SelectToken("supplemental.version")?.Parent?.Remove();
        }
    }
}
