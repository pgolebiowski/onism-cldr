using Newtonsoft.Json.Linq;
using Onism.Cldr.Tools.JsonParsers.Schemas;

namespace Onism.Cldr.Tools.JsonParsers
{
    /// <summary>
    /// Parses JSONs valid against the "Rbnf" schema.
    /// </summary>
    internal sealed class RbnfParser : CldrJsonParser
    {
        public RbnfParser() : base(CldrJsonSchemas.Rbnf)
        {
        }

        public override CldrJsonMetadata ExtractMetadata(JObject obj)
        {
            return new CldrJsonMetadata
            {
                CldrVersion = obj.SelectToken("rbnf.identity.version.cldrVersion").ToString(),
                CldrLocale = new CldrLocale(
                    language: obj.SelectToken("rbnf.identity.language").ToString(),
                    script: obj.SelectToken("rbnf.identity.script")?.ToString(),
                    territory: obj.SelectToken("rbnf.identity.territory")?.ToString(),
                    variant: obj.SelectToken("rbnf.identity.variant")?.ToString()
                )
            };
        }

        public override void RemoveMetadata(JObject obj)
        {
            obj.SelectToken("rbnf.identity")?.Parent?.Remove();
        }
    }
}
