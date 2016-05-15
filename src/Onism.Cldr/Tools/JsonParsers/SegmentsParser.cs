using Newtonsoft.Json.Linq;
using Onism.Cldr.Tools.JsonParsers.Schemas;

namespace Onism.Cldr.Tools.JsonParsers
{
    /// <summary>
    /// Parses JSONs valid against the "Segments" schema.
    /// </summary>
    internal sealed class SegmentsParser : CldrJsonParser
    {
        public SegmentsParser() : base(CldrJsonSchemas.Segments)
        {
        }

        public override CldrJsonMetadata ExtractMetadata(JObject obj)
        {
            return new CldrJsonMetadata
            {
                CldrVersion = obj.SelectToken("segments.identity.version._cldrVersion").ToString(),
                CldrLocale = new CldrLocale(
                    language: obj.SelectToken("segments.identity.language").ToString(),
                    script: obj.SelectToken("segments.identity.script")?.ToString(),
                    territory: obj.SelectToken("segments.identity.territory")?.ToString(),
                    variant: obj.SelectToken("segments.identity.variant")?.ToString()
                )
            };
        }

        public override void RemoveMetadata(JObject obj)
        {
            obj.SelectToken("segments.identity")?.Parent?.Remove();
        }
    }
}
