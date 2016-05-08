using Newtonsoft.Json.Linq;
using Onism.Cldr.JsonHandlers.Schemas;

namespace Onism.Cldr.JsonHandlers
{
    public class SegmentsHandler : CldrJsonHandler
    {
        public SegmentsHandler() : base(CldrJsonSchemas.Segments)
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
            obj.SelectToken("segments.identity").Parent.Remove();
        }
    }
}
