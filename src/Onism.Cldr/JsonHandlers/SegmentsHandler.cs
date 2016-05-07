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
                CldrLocale = new CldrLocale
                {
                    Language = obj.SelectToken("segments.identity.language").ToString(),
                    Script = obj.SelectToken("segments.identity.script")?.ToString(),
                    Territory = obj.SelectToken("segments.identity.territory")?.ToString(),
                    Variant = obj.SelectToken("segments.identity.variant")?.ToString()
                }
            };
        }

        public override void RemoveMetadata(JObject obj)
        {
            obj.SelectToken("segments.identity").Parent.Remove();
        }

        public override CldrJson PrepareForMerging(CldrLocale locale, JObject obj)
        {
            // data token is after subsetting and may be empty!
            var data = obj.SelectToken("segments.segmentations");
            if (data == null)
                return null;

            obj = new JObject(new JProperty("segments",
                new JObject(new JProperty(locale.Code, data))));

            return new CldrJson(locale, obj);
        }
    }
}
