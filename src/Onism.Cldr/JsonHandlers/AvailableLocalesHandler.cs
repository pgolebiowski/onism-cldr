using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Onism.Cldr.Extensions;
using Onism.Cldr.JsonHandlers.Schemas;

namespace Onism.Cldr.JsonHandlers
{
    public class AvailableLocalesHandler : CldrJsonHandler
    {
        public AvailableLocalesHandler() : base(CldrJsonSchemas.AvailableLocales)
        {
        }

        public override void Merge(CldrData cldrData, JToken token)
        {
            var modern = token.SelectTokens("availableLocales.modern").Children().Select(x => (string)x).ToHashSet();
            var full = token.SelectTokens("availableLocales.full").Children().Select(x => (string)x).ToArray();

            if (!modern.IsSubsetOf(full))
                throw new FormatException("AvailableLocales: 'modern' set is not a subset of the 'full' sel.");

            var availableLocales = full.Select(code => new AvailableLocale(code, modern.Contains(code)));
            cldrData.AvailableLocales = availableLocales;
        }
    }
}