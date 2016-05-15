using Newtonsoft.Json.Linq;
using Onism.Cldr.Tools.JsonParsers.Schemas;

namespace Onism.Cldr.Tools.JsonParsers
{
    /// <summary>
    /// Parses JSONs valid against the "Main" schema.
    /// </summary>
    internal sealed class MainParser : CldrJsonParser
    {
        public MainParser() : base(CldrJsonSchemas.Main)
        {
        }

        public override CldrJsonMetadata ExtractMetadata(JObject obj)
        {
            return new CldrJsonMetadata
            {
                CldrVersion = obj.SelectToken("main.*.identity.version._cldrVersion").ToString(),
                CldrLocale = new CldrLocale(
                    language: obj.SelectToken("main.*.identity.language").ToString(),
                    script: obj.SelectToken("main.*.identity.script")?.ToString(),
                    territory: obj.SelectToken("main.*.identity.territory")?.ToString(),
                    variant: obj.SelectToken("main.*.identity.variant")?.ToString()
                )
            };
        }

        public override void RemoveMetadata(JObject obj)
        {
            obj.SelectToken("main.*.identity").Parent.Remove();
        }

        public override CldrJson PrepareForMerging(CldrLocale locale, JObject obj)
        {
            var dataProperty = obj.SelectToken("main.*.*")?.Parent;
            if (dataProperty == null)
                return null;

            var data = new JObject((JProperty)dataProperty);
            return new CldrJson(locale, data);
        }
    }
}
