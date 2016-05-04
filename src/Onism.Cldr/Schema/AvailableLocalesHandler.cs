using System.Linq;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.Resources;

namespace Onism.Cldr.Schema
{
    public class AvailableLocalesHandler : CldrJsonHandler
    {
        public AvailableLocalesHandler() : base(CldrJsonSchemas.AvailableLocales)
        {
        }

        public override void Merge(CldrData cldrData, JToken token)
        {
            var modern = token.SelectTokens("availableLocales.modern").Children().Select(x => (string)x).ToHashSet();
            var full = token.SelectTokens("availableLocales.full").Children().Select(x => (string)x);

            // TODO: maybe some assert that modern is a subset of full?

            var availableLocales = full.Select(code => new AvailableLocale(code, modern.Contains(code)));
            cldrData.AvailableLocales = availableLocales;
        }
    }
}