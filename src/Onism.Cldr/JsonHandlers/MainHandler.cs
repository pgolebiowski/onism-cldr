using Newtonsoft.Json.Linq;
using Onism.Cldr.JsonHandlers.Schemas;

namespace Onism.Cldr.JsonHandlers
{
    public class MainHandler : CldrJsonHandler
    {
        public MainHandler() : base(CldrJsonSchemas.Main)
        {
        }

        public override CldrJsonMetadata ExtractMetadata(JObject obj)
        {
            return new CldrJsonMetadata
            {
                CldrVersion = obj.SelectToken("main.*.identity.version._cldrVersion").ToString(),
                CldrLocale = new CldrLocale
                {
                    Language = obj.SelectToken("main.*.identity.language").ToString(),
                    Script = obj.SelectToken("main.*.identity.script")?.ToString(),
                    Territory = obj.SelectToken("main.*.identity.territory")?.ToString(),
                    Variant = obj.SelectToken("main.*.identity.variant")?.ToString()
                }
            };
        }

        public override void RemoveMetadata(JObject obj)
        {
            obj.SelectToken("main.*.identity").Parent.Remove();
        }

        public override CldrJson PrepareForMerging(CldrLocale locale, JObject obj)
        {
            // data token is after subsetting and may be empty!
            var dataProperty = obj.SelectToken("main.*.*")?.Parent;
            if (dataProperty == null)
                return null;

            var data = new JObject((JProperty)dataProperty);
            return new CldrJson(locale, data);
        }
    }
}
