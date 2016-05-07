using Newtonsoft.Json.Linq;
using Onism.Cldr.JsonHandlers.Schemas;

namespace Onism.Cldr.JsonHandlers
{
    public class RbnfHandler : CldrJsonHandler
    {
        public RbnfHandler() : base(CldrJsonSchemas.Rbnf)
        {
        }

        public override CldrJsonMetadata ExtractMetadata(JObject obj)
        {
            return new CldrJsonMetadata
            {
                CldrVersion = obj.SelectToken("rbnf.identity.version.cldrVersion").ToString(),
                CldrLocale = new CldrLocale
                {
                    Language = obj.SelectToken("rbnf.identity.language").ToString(),
                    Script = obj.SelectToken("rbnf.identity.script")?.ToString(),
                    Territory = obj.SelectToken("rbnf.identity.territory")?.ToString(),
                    Variant = obj.SelectToken("rbnf.identity.variant")?.ToString()
                }
            };
        }

        public override void RemoveMetadata(JObject obj)
        {
            obj.SelectToken("rbnf.identity").Parent.Remove();
        }
    }
}
